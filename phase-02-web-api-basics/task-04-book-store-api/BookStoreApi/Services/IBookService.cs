using BookStoreApi.DTOs;

namespace BookStoreApi.Services;

public interface IBookService
{
    Task<PagedResult<BookResponse>> GetPagedBooksAsync(BookFilterQuery query);
    Task<BookResponse?> GetBookByIdAsync(int id);
    Task<(bool Success, string? Error, BookResponse? Data)> CreateBookAsync(CreateBookRequest request);
    Task<(bool Success, string? Error, BookResponse? Data)> UpdateBookAsync(int id, UpdateBookRequest request);
    Task<(bool Success, string? Error)> DeleteBookAsync(int id);
    Task<BookStoreReportResponse> GetReportSummaryAsync();
    Task<bool> HasBooksForAuthorAsync(int authorId);
    Task<bool> HasBooksForCategoryAsync(int categoryId);
    Task<int> GetBookCountForAuthorAsync(int authorId);
    Task<int> GetBookCountForCategoryAsync(int categoryId);
}
