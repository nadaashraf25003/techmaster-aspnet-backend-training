namespace BookStoreApi.DTOs;

public class CategoryReportDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BookCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class AuthorReportDto
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int BookCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class BookStoreReportResponse
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int OutOfStockBooks { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public IEnumerable<CategoryReportDto> BooksPerCategory { get; set; } = Enumerable.Empty<CategoryReportDto>();
    public IEnumerable<AuthorReportDto> BooksPerAuthor { get; set; } = Enumerable.Empty<AuthorReportDto>();
}
