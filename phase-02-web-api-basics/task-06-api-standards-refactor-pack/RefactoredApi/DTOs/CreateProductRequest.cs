using System.ComponentModel.DataAnnotations;

namespace RefactoredApi.DTOs;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int Stock { get; set; }
}
