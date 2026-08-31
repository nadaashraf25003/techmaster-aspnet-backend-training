using System.ComponentModel.DataAnnotations;

namespace ProductsCategoriesApi.DTOs;

/// <summary>
/// Request payload for creating a new product category.
/// </summary>
public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request payload for updating an existing category.
/// </summary>
public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "IsActive status is required.")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Response representation of a category.
/// </summary>
public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductsCount { get; set; }
}
