using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Services;

public class AuthorService : IAuthorService
{
    private readonly List<Author> _authors = new();
    private readonly Lock _lock = new();
    private int _nextId = 1;

    public AuthorService()
    {
        SeedInitialAuthors();
    }

    private void SeedInitialAuthors()
    {
        var seeds = new List<Author>
        {
            new()
            {
                AuthorId = _nextId++,
                FullName = "George Orwell",
                Bio = "English novelist, essayist, journalist, and critic, famous for 1984 and Animal Farm.",
                CreatedAt = DateTime.UtcNow.AddMonths(-12)
            },
            new()
            {
                AuthorId = _nextId++,
                FullName = "J.K. Rowling",
                Bio = "British author, best known for writing the Harry Potter fantasy series.",
                CreatedAt = DateTime.UtcNow.AddMonths(-10)
            },
            new()
            {
                AuthorId = _nextId++,
                FullName = "J.R.R. Tolkien",
                Bio = "English writer, poet, philologist, and academic, author of The Hobbit and The Lord of the Rings.",
                CreatedAt = DateTime.UtcNow.AddMonths(-8)
            }
        };

        _authors.AddRange(seeds);
    }

    public Task<IEnumerable<AuthorResponse>> GetAllAuthorsAsync()
    {
        lock (_lock)
        {
            var result = _authors
                .OrderBy(a => a.AuthorId)
                .Select(a => MapToDto(a))
                .ToList();

            return Task.FromResult<IEnumerable<AuthorResponse>>(result);
        }
    }

    public Task<AuthorResponse?> GetAuthorByIdAsync(int id)
    {
        lock (_lock)
        {
            var author = _authors.FirstOrDefault(a => a.AuthorId == id);
            return Task.FromResult(author != null ? MapToDto(author) : null);
        }
    }

    public Task<bool> AuthorExistsAsync(int id)
    {
        lock (_lock)
        {
            var exists = _authors.Any(a => a.AuthorId == id);
            return Task.FromResult(exists);
        }
    }

    public Task<string?> GetAuthorNameAsync(int id)
    {
        lock (_lock)
        {
            var name = _authors.FirstOrDefault(a => a.AuthorId == id)?.FullName;
            return Task.FromResult(name);
        }
    }

    public Task<(bool Success, string? Error, AuthorResponse? Data)> CreateAuthorAsync(CreateAuthorRequest request)
    {
        lock (_lock)
        {
            var fullName = request.FullName.Trim();

            var author = new Author
            {
                AuthorId = _nextId++,
                FullName = fullName,
                Bio = request.Bio?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _authors.Add(author);
            return Task.FromResult<(bool, string?, AuthorResponse?)>((true, null, MapToDto(author)));
        }
    }

    public Task<(bool Success, string? Error, AuthorResponse? Data)> UpdateAuthorAsync(int id, UpdateAuthorRequest request)
    {
        lock (_lock)
        {
            var author = _authors.FirstOrDefault(a => a.AuthorId == id);
            if (author == null)
            {
                return Task.FromResult<(bool, string?, AuthorResponse?)>((
                    false,
                    $"Author with ID {id} was not found.",
                    null
                ));
            }

            author.FullName = request.FullName.Trim();
            author.Bio = request.Bio?.Trim();

            return Task.FromResult<(bool, string?, AuthorResponse?)>((true, null, MapToDto(author)));
        }
    }

    public Task<(bool Success, string? Error)> DeleteAuthorAsync(int id)
    {
        lock (_lock)
        {
            var author = _authors.FirstOrDefault(a => a.AuthorId == id);
            if (author == null)
            {
                return Task.FromResult<(bool, string?)>((false, $"Author with ID {id} was not found."));
            }

            _authors.Remove(author);
            return Task.FromResult<(bool, string?)>((true, null));
        }
    }

    private static AuthorResponse MapToDto(Author author, int booksCount = 0)
    {
        return new AuthorResponse
        {
            AuthorId = author.AuthorId,
            FullName = author.FullName,
            Bio = author.Bio,
            CreatedAt = author.CreatedAt,
            BooksCount = booksCount
        };
    }
}
