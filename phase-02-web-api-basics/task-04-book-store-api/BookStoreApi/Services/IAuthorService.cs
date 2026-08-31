using BookStoreApi.DTOs;

namespace BookStoreApi.Services;

public interface IAuthorService
{
    Task<IEnumerable<AuthorResponse>> GetAllAuthorsAsync();
    Task<AuthorResponse?> GetAuthorByIdAsync(int id);
    Task<(bool Success, string? Error, AuthorResponse? Data)> CreateAuthorAsync(CreateAuthorRequest request);
    Task<(bool Success, string? Error, AuthorResponse? Data)> UpdateAuthorAsync(int id, UpdateAuthorRequest request);
    Task<(bool Success, string? Error)> DeleteAuthorAsync(int id);
    Task<bool> AuthorExistsAsync(int id);
    Task<string?> GetAuthorNameAsync(int id);
}
