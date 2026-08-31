using ProductsCategoriesApi.DTOs;
using ProductsCategoriesApi.Models;

namespace ProductsCategoriesApi.Services;

/// <summary>
/// Thread-safe in-memory implementation of category management.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly List<Category> _categories = new();
    private readonly Lock _lock = new();
    private int _nextId = 1;

    public CategoryService()
    {
        SeedInitialCategories();
    }

    private void SeedInitialCategories()
    {
        var seeds = new List<Category>
        {
            new()
            {
                CategoryId = _nextId++,
                Name = "Electronics",
                Description = "High-tech gadgets, computer hardware, and peripherals.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            },
            new()
            {
                CategoryId = _nextId++,
                Name = "Furniture",
                Description = "Ergonomic office desks, chairs, and workspace lighting.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            },
            new()
            {
                CategoryId = _nextId++,
                Name = "Stationery",
                Description = "Notebooks, pens, writing materials, and office paper supplies.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                CategoryId = _nextId++,
                Name = "Accessories",
                Description = "Backpacks, sleeves, mouse pads, and personal computing accessories.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            }
        };

        _categories.AddRange(seeds);
    }

    public Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync(bool includeInactive = false)
    {
        lock (_lock)
        {
            var query = _categories.AsEnumerable();
            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            var result = query
                .OrderBy(c => c.CategoryId)
                .Select(c => MapToDto(c))
                .ToList();

            return Task.FromResult<IEnumerable<CategoryResponse>>(result);
        }
    }

    public Task<CategoryResponse?> GetCategoryByIdAsync(int id)
    {
        lock (_lock)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryId == id);
            return Task.FromResult(category != null ? MapToDto(category) : null);
        }
    }

    public Task<bool> CategoryExistsAndActiveAsync(int id)
    {
        lock (_lock)
        {
            var exists = _categories.Any(c => c.CategoryId == id && c.IsActive);
            return Task.FromResult(exists);
        }
    }

    public Task<bool> CategoryExistsAsync(int id)
    {
        lock (_lock)
        {
            var exists = _categories.Any(c => c.CategoryId == id);
            return Task.FromResult(exists);
        }
    }

    public Task<string?> GetCategoryNameAsync(int id)
    {
        lock (_lock)
        {
            var name = _categories.FirstOrDefault(c => c.CategoryId == id)?.Name;
            return Task.FromResult(name);
        }
    }

    public Task<(bool Success, string? Error, CategoryResponse? Data)> CreateCategoryAsync(CreateCategoryRequest request)
    {
        lock (_lock)
        {
            var name = request.Name.Trim();
            if (_categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult<(bool, string?, CategoryResponse?)>((
                    false,
                    $"A category with the name '{request.Name}' already exists.",
                    null
                ));
            }

            var category = new Category
            {
                CategoryId = _nextId++,
                Name = name,
                Description = request.Description?.Trim(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _categories.Add(category);
            return Task.FromResult<(bool, string?, CategoryResponse?)>((true, null, MapToDto(category)));
        }
    }

    public Task<(bool Success, string? Error, CategoryResponse? Data)> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        lock (_lock)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return Task.FromResult<(bool, string?, CategoryResponse?)>((
                    false,
                    $"Category with ID {id} was not found.",
                    null
                ));
            }

            var name = request.Name.Trim();
            if (_categories.Any(c => c.CategoryId != id && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult<(bool, string?, CategoryResponse?)>((
                    false,
                    $"Another category with the name '{request.Name}' already exists.",
                    null
                ));
            }

            category.Name = name;
            category.Description = request.Description?.Trim();
            category.IsActive = request.IsActive;

            return Task.FromResult<(bool, string?, CategoryResponse?)>((true, null, MapToDto(category)));
        }
    }

    public Task<(bool Success, string? Error)> DeleteCategoryAsync(int id)
    {
        lock (_lock)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return Task.FromResult<(bool, string?)>((false, $"Category with ID {id} was not found."));
            }

            _categories.Remove(category);
            return Task.FromResult<(bool, string?)>((true, null));
        }
    }

    private static CategoryResponse MapToDto(Category category, int productsCount = 0)
    {
        return new CategoryResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            ProductsCount = productsCount
        };
    }
}
