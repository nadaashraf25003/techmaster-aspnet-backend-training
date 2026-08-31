namespace ProductsCategoriesApi.Models;

/// <summary>
/// Domain model representing a product category in the store inventory.
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
