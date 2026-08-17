namespace ProductCatalog.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public bool IsInStock => StockQuantity > 0;
    public bool IsAvailable => IsInStock;
    public decimal TotalStockValue => Price * StockQuantity;

    public override string ToString()
    {
        return $"[{Id}] {Name} | Category: {Category} | Price: ${Price:F2} | Stock: {StockQuantity} | Supplier: {Supplier} | Created: {CreatedAt:yyyy-MM-dd}";
    }
}
