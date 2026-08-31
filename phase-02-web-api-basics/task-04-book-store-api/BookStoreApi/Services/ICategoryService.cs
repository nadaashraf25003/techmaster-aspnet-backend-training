using BookStoreApi.DTOs;

namespace BookStoreApi.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync(bool includeInactive = false);
    Task<CategoryResponse?> GetCategoryByIdAsync(int id);
    Task<(bool Success, string? Error, CategoryResponse? Data)> CreateCategoryAsync(CreateCategoryRequest request);
    Task<(bool Success, string? Error, CategoryResponse? Data)> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<(bool Success, string? Error)> DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(int id);
    Task<bool> IsCategoryActiveAsync(int id);
    Task<string?> GetCategoryNameAsync(int id);
}
