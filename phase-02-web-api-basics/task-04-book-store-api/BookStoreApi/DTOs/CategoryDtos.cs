using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.DTOs;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}


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

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BooksCount { get; set; }
}
