using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Services;

public class BookService : IBookService
{
    private readonly List<Book> _books = new();
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;
    private readonly Lock _lock = new();
    private int _nextId = 1;

    public BookService(IAuthorService authorService, ICategoryService categoryService)
    {
        _authorService = authorService;
        _categoryService = categoryService;
        SeedInitialBooks();
    }

    private void SeedInitialBooks()
    {
        var seeds = new List<Book>
        {
            new()
            {
                BookId = _nextId++,
                Title = "1984",
                ISBN = "9780451524935",
                Price = 9.99m,
                StockQuantity = 10,
                AuthorId = 1, // George Orwell
                CategoryId = 3, // Science Fiction
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                BookId = _nextId++,
                Title = "Animal Farm",
                ISBN = "9780451526342",
                Price = 7.99m,
                StockQuantity = 0, // Out of Stock
                AuthorId = 1, // George Orwell
                CategoryId = 1, // Fiction
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                BookId = _nextId++,
                Title = "Harry Potter and the Sorcerer's Stone",
                ISBN = "9780590353427",
                Price = 12.99m,
                StockQuantity = 25,
                AuthorId = 2, // J.K. Rowling
                CategoryId = 2, // Fantasy
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                BookId = _nextId++,
                Title = "The Hobbit",
                ISBN = "9780547928227",
                Price = 14.99m,
                StockQuantity = 15,
                AuthorId = 3, // J.R.R. Tolkien
                CategoryId = 2, // Fantasy
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            }
        };

        _books.AddRange(seeds);
    }

    public async Task<PagedResult<BookResponse>> GetPagedBooksAsync(BookFilterQuery query)
    {
        List<Book> snapshot;
        lock (_lock)
        {
            snapshot = _books.ToList();
        }

        var filtered = snapshot.AsEnumerable();

        // 1. Search term (Title or ISBN)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            filtered = filtered.Where(b =>
                b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Filter by CategoryId
        if (query.CategoryId.HasValue)
        {
            filtered = filtered.Where(b => b.CategoryId == query.CategoryId.Value);
        }

        // 3. Filter by AuthorId
        if (query.AuthorId.HasValue)
        {
            filtered = filtered.Where(b => b.AuthorId == query.AuthorId.Value);
        }

        // 4. Filter by Availability (Stock > 0)
        if (query.IsAvailable.HasValue)
        {
            filtered = filtered.Where(b => b.IsAvailable == query.IsAvailable.Value);
        }

        var resultList = filtered.OrderBy(b => b.BookId).ToList();
        int totalCount = resultList.Count;

        // Apply pagination
        int pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        int pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var pagedItems = resultList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var responseList = new List<BookResponse>();
        foreach (var b in pagedItems)
        {
            var authorName = await _authorService.GetAuthorNameAsync(b.AuthorId) ?? "Unknown";
            var categoryName = await _categoryService.GetCategoryNameAsync(b.CategoryId) ?? "Unknown";
            responseList.Add(MapToDto(b, authorName, categoryName));
        }

        return new PagedResult<BookResponse>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = responseList
        };
    }

    public async Task<BookResponse?> GetBookByIdAsync(int id)
    {
        Book? book;
        lock (_lock)
        {
            book = _books.FirstOrDefault(b => b.BookId == id);
        }

        if (book == null) return null;

        var authorName = await _authorService.GetAuthorNameAsync(book.AuthorId) ?? "Unknown";
        var categoryName = await _categoryService.GetCategoryNameAsync(book.CategoryId) ?? "Unknown";
        return MapToDto(book, authorName, categoryName);
    }

    public async Task<(bool Success, string? Error, BookResponse? Data)> CreateBookAsync(CreateBookRequest request)
    {
        // 1. Author existence validation
        var authorExists = await _authorService.AuthorExistsAsync(request.AuthorId);
        if (!authorExists)
        {
            return (false, $"Author with ID {request.AuthorId} does not exist.", null);
        }

        // 2. Category existence validation
        var categoryExists = await _categoryService.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return (false, $"Category with ID {request.CategoryId} does not exist.", null);
        }

        // 3. Category active validation (Business rule: Inactive categories should not be used for new books)
        var categoryActive = await _categoryService.IsCategoryActiveAsync(request.CategoryId);
        if (!categoryActive)
        {
            return (false, $"Category with ID {request.CategoryId} is inactive and cannot be assigned to new books.", null);
        }

        var isbn = request.ISBN.Trim();
        var authorName = await _authorService.GetAuthorNameAsync(request.AuthorId) ?? "Unknown";
        var categoryName = await _categoryService.GetCategoryNameAsync(request.CategoryId) ?? "Unknown";

        lock (_lock)
        {
            // 4. ISBN uniqueness validation (case-insensitive)
            if (_books.Any(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, $"A book with ISBN '{request.ISBN}' already exists.", null);
            }

            var book = new Book
            {
                BookId = _nextId++,
                Title = request.Title.Trim(),
                ISBN = isbn,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                AuthorId = request.AuthorId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _books.Add(book);
            return (true, null, MapToDto(book, authorName, categoryName));
        }
    }

    public async Task<(bool Success, string? Error, BookResponse? Data)> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        // 1. Author existence validation
        var authorExists = await _authorService.AuthorExistsAsync(request.AuthorId);
        if (!authorExists)
        {
            return (false, $"Author with ID {request.AuthorId} does not exist.", null);
        }

        // 2. Category existence validation
        var categoryExists = await _categoryService.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return (false, $"Category with ID {request.CategoryId} does not exist.", null);
        }

        // 3. Category active validation
        var categoryActive = await _categoryService.IsCategoryActiveAsync(request.CategoryId);
        if (!categoryActive)
        {
            return (false, $"Category with ID {request.CategoryId} is inactive and cannot be assigned to books.", null);
        }

        var isbn = request.ISBN.Trim();
        var authorName = await _authorService.GetAuthorNameAsync(request.AuthorId) ?? "Unknown";
        var categoryName = await _categoryService.GetCategoryNameAsync(request.CategoryId) ?? "Unknown";

        lock (_lock)
        {
            var book = _books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return (false, $"Book with ID {id} was not found.", null);
            }

            // 4. ISBN uniqueness validation
            if (_books.Any(b => b.BookId != id && b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, $"Another book with ISBN '{request.ISBN}' already exists.", null);
            }

            book.Title = request.Title.Trim();
            book.ISBN = isbn;
            book.Price = request.Price;
            book.StockQuantity = request.StockQuantity;
            book.AuthorId = request.AuthorId;
            book.CategoryId = request.CategoryId;
            book.UpdatedAt = DateTime.UtcNow;

            return (true, null, MapToDto(book, authorName, categoryName));
        }
    }

    public Task<(bool Success, string? Error)> DeleteBookAsync(int id)
    {
        lock (_lock)
        {
            var book = _books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return Task.FromResult<(bool, string?)>((false, $"Book with ID {id} was not found."));
            }

            _books.Remove(book);
            return Task.FromResult<(bool, string?)>((true, null));
        }
    }

    public async Task<BookStoreReportResponse> GetReportSummaryAsync()
    {
        List<Book> snapshot;
        lock (_lock)
        {
            snapshot = _books.ToList();
        }

        var authors = (await _authorService.GetAllAuthorsAsync()).ToList();
        var categories = (await _categoryService.GetAllCategoriesAsync(includeInactive: true)).ToList();

        var totalBooks = snapshot.Count;
        var availableBooks = snapshot.Count(b => b.IsAvailable);
        var outOfStockBooks = snapshot.Count(b => b.StockQuantity == 0);
        var totalVal = snapshot.Sum(b => b.Price * b.StockQuantity);

        var booksPerCategory = new List<CategoryReportDto>();
        foreach (var cat in categories)
        {
            var catBooks = snapshot.Where(b => b.CategoryId == cat.CategoryId).ToList();
            booksPerCategory.Add(new CategoryReportDto
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.Name,
                BookCount = catBooks.Count,
                TotalValue = Math.Round(catBooks.Sum(b => b.Price * b.StockQuantity), 2)
            });
        }

        var booksPerAuthor = new List<AuthorReportDto>();
        foreach (var auth in authors)
        {
            var authBooks = snapshot.Where(b => b.AuthorId == auth.AuthorId).ToList();
            booksPerAuthor.Add(new AuthorReportDto
            {
                AuthorId = auth.AuthorId,
                AuthorName = auth.FullName,
                BookCount = authBooks.Count,
                TotalValue = Math.Round(authBooks.Sum(b => b.Price * b.StockQuantity), 2)
            });
        }

        return new BookStoreReportResponse
        {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            OutOfStockBooks = outOfStockBooks,
            TotalInventoryValue = Math.Round(totalVal, 2),
            BooksPerCategory = booksPerCategory,
            BooksPerAuthor = booksPerAuthor
        };
    }

    public Task<bool> HasBooksForAuthorAsync(int authorId)
    {
        lock (_lock)
        {
            return Task.FromResult(_books.Any(b => b.AuthorId == authorId));
        }
    }

    public Task<bool> HasBooksForCategoryAsync(int categoryId)
    {
        lock (_lock)
        {
            return Task.FromResult(_books.Any(b => b.CategoryId == categoryId));
        }
    }

    public Task<int> GetBookCountForAuthorAsync(int authorId)
    {
        lock (_lock)
        {
            return Task.FromResult(_books.Count(b => b.AuthorId == authorId));
        }
    }

    public Task<int> GetBookCountForCategoryAsync(int categoryId)
    {
        lock (_lock)
        {
            return Task.FromResult(_books.Count(b => b.CategoryId == categoryId));
        }
    }

    private static BookResponse MapToDto(Book b, string authorName, string categoryName)
    {
        return new BookResponse
        {
            BookId = b.BookId,
            Title = b.Title,
            ISBN = b.ISBN,
            Price = b.Price,
            StockQuantity = b.StockQuantity,
            IsAvailable = b.IsAvailable,
            AuthorId = b.AuthorId,
            AuthorName = authorName,
            CategoryId = b.CategoryId,
            CategoryName = categoryName,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };
    }
}
