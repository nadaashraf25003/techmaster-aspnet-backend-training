using ProductsCategoriesApi.DTOs;

namespace ProductsCategoriesApi.Services;

/// <summary>
/// Service contract for managing store products, inventory stock, search/filters, and reporting.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Returns products matching search and multi-criteria filters.
    /// </summary>
    Task<IEnumerable<ProductResponse>> GetFilteredProductsAsync(ProductFilterQuery query);

    /// <summary>
    /// Gets a single product by unique ID.
    /// </summary>
    Task<ProductResponse?> GetProductByIdAsync(int id);

    /// <summary>
    /// Creates a new product with category verification.
    /// </summary>
    Task<(bool Success, string? Error, ProductResponse? Data)> CreateProductAsync(CreateProductRequest request);

    /// <summary>
    /// Updates product details with category verification.
    /// </summary>
    Task<(bool Success, string? Error, ProductResponse? Data)> UpdateProductAsync(int id, UpdateProductRequest request);

    /// <summary>
    /// Updates stock quantity for a specific product.
    /// </summary>
    Task<(bool Success, string? Error, ProductResponse? Data)> UpdateStockAsync(int id, int stockQuantity);

    /// <summary>
    /// Deletes a product or marks it unavailable.
    /// </summary>
    Task<(bool Success, string? Error)> DeleteProductAsync(int id);

    /// <summary>
    /// Gets products that have stock less than or equal to the specified threshold.
    /// </summary>
    Task<IEnumerable<ProductResponse>> GetLowStockProductsAsync(int threshold = 5);

    /// <summary>
    /// Generates aggregate stock valuation and category reports using LINQ.
    /// </summary>
    Task<StockReportResponse> GetStockReportAsync();

    /// <summary>
    /// Checks if any product is associated with a given category ID.
    /// </summary>
    Task<bool> HasProductsForCategoryAsync(int categoryId);

    /// <summary>
    /// Returns count of products belonging to a given category ID.
    /// </summary>
    Task<int> GetProductCountForCategoryAsync(int categoryId);
}
