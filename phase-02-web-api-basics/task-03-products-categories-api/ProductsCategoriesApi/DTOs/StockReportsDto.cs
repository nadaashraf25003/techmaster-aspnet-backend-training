namespace ProductsCategoriesApi.DTOs;

/// <summary>
/// Stock summary per category.
/// </summary>
public class CategoryStockSummaryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public int TotalUnitsInStock { get; set; }
    public decimal TotalCategoryValue { get; set; }
}

/// <summary>
/// Comprehensive stock valuation and inventory report.
/// </summary>
public class StockReportResponse
{
    public decimal TotalStockValue { get; set; }
    public int TotalProducts { get; set; }
    public int TotalUnitsInStock { get; set; }
    public int LowStockProductsCount { get; set; }
    public int OutOfStockProductsCount { get; set; }
    public List<CategoryStockSummaryDto> StockValuePerCategory { get; set; } = new();
    public List<ProductResponse> LowStockProducts { get; set; } = new();
    public List<ProductResponse> OutOfStockProducts { get; set; } = new();
}
