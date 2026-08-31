using ProductsCategoriesApi.DTOs;

namespace ProductsCategoriesApi.Services;

/// <summary>
/// Service contract for managing product categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Returns all categories. Inactive categories are excluded unless includeInactive is true.
    /// </summary>
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync(bool includeInactive = false);

    /// <summary>
    /// Gets category by ID.
    /// </summary>
    Task<CategoryResponse?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Checks if a category exists and is active.
    /// </summary>
    Task<bool> CategoryExistsAndActiveAsync(int id);

    /// <summary>
    /// Checks if a category ID exists.
    /// </summary>
    Task<bool> CategoryExistsAsync(int id);

    /// <summary>
    /// Gets category name by ID.
    /// </summary>
    Task<string?> GetCategoryNameAsync(int id);

    /// <summary>
    /// Creates a new category ensuring unique name.
    /// </summary>
    Task<(bool Success, string? Error, CategoryResponse? Data)> CreateCategoryAsync(CreateCategoryRequest request);

    /// <summary>
    /// Updates category details ensuring unique name across others.
    /// </summary>
    Task<(bool Success, string? Error, CategoryResponse? Data)> UpdateCategoryAsync(int id, UpdateCategoryRequest request);

    /// <summary>
    /// Deletes a category.
    /// </summary>
    Task<(bool Success, string? Error)> DeleteCategoryAsync(int id);
}
