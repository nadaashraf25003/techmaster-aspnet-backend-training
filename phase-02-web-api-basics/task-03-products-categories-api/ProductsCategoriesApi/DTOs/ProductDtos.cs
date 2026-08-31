using System.ComponentModel.DataAnnotations;

namespace ProductsCategoriesApi.DTOs;

/// <summary>
/// Request payload for creating a new product.
/// </summary>
public class CreateProductRequest
{
    [Required(ErrorMessage = "Product Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Product Name must be between 2 and 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000.0, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "StockQuantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity cannot be negative.")]
    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Required(ErrorMessage = "SupplierName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "SupplierName must be between 2 and 100 characters.")]
    public string SupplierName { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for updating an existing product.
/// </summary>
public class UpdateProductRequest
{
    [Required(ErrorMessage = "Product Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Product Name must be between 2 and 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000.0, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "StockQuantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity cannot be negative.")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "IsAvailable status is required.")]
    public bool IsAvailable { get; set; }

    [Required(ErrorMessage = "SupplierName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "SupplierName must be between 2 and 100 characters.")]
    public string SupplierName { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for patching product stock quantity.
/// </summary>
public class UpdateStockRequest
{
    [Required(ErrorMessage = "StockQuantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity cannot be negative.")]
    public int StockQuantity { get; set; }
}

/// <summary>
/// Response representation of a product.
/// </summary>
public class ProductResponse
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Query parameters for product search and multi-criteria filtering.
/// </summary>
public class ProductFilterQuery
{
    /// <summary>
    /// Search term matched against product Name or SupplierName (case-insensitive substring).
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter products belonging to a specific Category ID.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter products with price greater than or equal to this amount.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "MinPrice must be non-negative.")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Filter products with price less than or equal to this amount.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "MaxPrice must be non-negative.")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Filter by product availability status.
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// Filter products that have low stock (<= threshold).
    /// </summary>
    public bool? IsLowStock { get; set; }

    /// <summary>
    /// Threshold value to consider a product as low stock (default is 5).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "LowStockThreshold must be at least 1.")]
    public int LowStockThreshold { get; set; } = 5;
}
