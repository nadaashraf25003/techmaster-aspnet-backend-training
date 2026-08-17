namespace ProductCatalog.Models;

public class SupplierReport
{
    public string SupplierName { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal AveragePrice { get; set; }
}
