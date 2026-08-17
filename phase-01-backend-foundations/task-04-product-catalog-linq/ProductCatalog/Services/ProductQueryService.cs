using ProductCatalog.Models;

namespace ProductCatalog.Services;

public class ProductQueryService
{
    private readonly List<Product> _products;

    public ProductQueryService(IEnumerable<Product> products)
    {
        _products = products?.ToList() ?? new List<Product>();
    }

    /// <summary>
    /// Returns all products in the catalog.
    /// </summary>
    public IReadOnlyList<Product> GetAllProducts()
    {
        return _products.ToList();
    }

    /// <summary>
    /// Menu 1 / Query 1: Returns all products currently in stock (StockQuantity > 0).
    /// Required LINQ Concept: Where
    /// </summary>
    public IEnumerable<Product> GetAvailableProducts()
    {
        return _products.Where(p => p.IsInStock);
    }

    /// <summary>
    /// Menu 2 / Query 2: Filters products belonging to a specified category (case-insensitive).
    /// Required LINQ Concept: Where + StringComparison
    /// </summary>
    public IEnumerable<Product> FilterByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return _products;

        return _products.Where(p => string.Equals(p.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Menu 3 / Query 3: Filters products within a specific price range.
    /// Required LINQ Concept: Where
    /// </summary>
    public IEnumerable<Product> FilterByPriceRange(decimal minPrice, decimal maxPrice)
    {
        if (minPrice > maxPrice)
            (minPrice, maxPrice) = (maxPrice, minPrice);

        return _products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
    }

    /// <summary>
    /// Menu 4 / Query 4: Searches products by partial name match (case-insensitive).
    /// Required LINQ Concept: Where + Contains / IndexOf
    /// </summary>
    public IEnumerable<Product> SearchByName(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return _products;

        return _products.Where(p => p.Name.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Menu 5 / Query 5: Sorts products by price in ascending or descending order.
    /// Required LINQ Concept: OrderBy / OrderByDescending
    /// </summary>
    public IEnumerable<Product> SortByPrice(bool ascending = true)
    {
        return ascending
            ? _products.OrderBy(p => p.Price)
            : _products.OrderByDescending(p => p.Price);
    }

    /// <summary>
    /// Menu 6 / Query 6: Groups products by their category.
    /// Required LINQ Concept: GroupBy
    /// </summary>
    public IEnumerable<IGrouping<string, Product>> GroupByCategory()
    {
        return _products.GroupBy(p => p.Category)
                        .OrderBy(g => g.Key);
    }

    /// <summary>
    /// Menu 7 / Query 17: Calculates statistical aggregates per category (Count, Average, Max, Min, Total Stock Value).
    /// Required LINQ Concept: GroupBy + Select
    /// Safely handles empty collections.
    /// </summary>
    public IEnumerable<CategoryStats> GetCategoryStatistics()
    {
        return _products
            .GroupBy(p => p.Category)
            .Select(g => new CategoryStats
            {
                Category = g.Key,
                ProductCount = g.Count(),
                AveragePrice = g.Any() ? g.Average(p => p.Price) : 0m,
                MaxPrice = g.Any() ? g.Max(p => p.Price) : 0m,
                MinPrice = g.Any() ? g.Min(p => p.Price) : 0m,
                TotalStockValue = g.Sum(p => p.Price * p.StockQuantity)
            })
            .OrderBy(s => s.Category);
    }

    /// <summary>
    /// Menu 8 / Query 8: Returns products with stock quantity below or equal to the threshold.
    /// Required LINQ Concept: Where + OrderBy
    /// </summary>
    public IEnumerable<Product> GetLowStockProducts(int threshold = 5)
    {
        return _products
            .Where(p => p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity);
    }

    /// <summary>
    /// Menu 9 / Query 15: Generates a supplier report grouping by supplier and calculating product count, total stock value, and average price.
    /// Required LINQ Concept: GroupBy + Select
    /// </summary>
    public IEnumerable<SupplierReport> GetSupplierReport()
    {
        return _products
            .GroupBy(p => p.Supplier)
            .Select(g => new SupplierReport
            {
                SupplierName = g.Key,
                TotalProducts = g.Count(),
                TotalStockValue = g.Sum(p => p.Price * p.StockQuantity),
                AveragePrice = g.Any() ? g.Average(p => p.Price) : 0m
            })
            .OrderByDescending(r => r.TotalStockValue);
    }

    /// <summary>
    /// Menu 10 / Query 20: Returns a paginated slice of products based on page number and page size.
    /// Required LINQ Concept: Skip + Take
    /// </summary>
    public PagedResult<Product> GetPagedProducts(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 5;

        var totalItems = _products.Count;
        var pagedItems = _products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PagedResult<Product>(pagedItems, pageNumber, pageSize, totalItems);
    }

    /// <summary>
    /// Query 16: Returns products created in the last N days (default 60 days).
    /// Required LINQ Concept: Where + DateTime
    /// </summary>
    public IEnumerable<Product> GetRecentlyAddedProducts(int days = 60, DateTime? referenceDate = null)
    {
        var targetBase = referenceDate ?? (_products.Count > 0 ? _products.Max(p => p.CreatedAt) : DateTime.Today);
        var targetDate = targetBase.AddDays(-days);
        return _products
            .Where(p => p.CreatedAt >= targetDate)
            .OrderByDescending(p => p.CreatedAt);
    }

    /// <summary>
    /// Query 18: Returns products with price strictly above the overall catalog average.
    /// Required LINQ Concept: Average + Where (Two-step query)
    /// </summary>
    public (decimal AveragePrice, IEnumerable<Product> Products) GetProductsAboveAveragePrice()
    {
        if (_products.Count == 0)
            return (0m, Enumerable.Empty<Product>());

        var averagePrice = _products.Average(p => p.Price);
        var productsAboveAvg = _products
            .Where(p => p.Price > averagePrice)
            .OrderByDescending(p => p.Price);

        return (averagePrice, productsAboveAvg);
    }

    /// <summary>
    /// Query 19: Combines keyword search, category filtering, price range, and availability using chained Where queries.
    /// Required LINQ Concept: Chained Where
    /// </summary>
    public IEnumerable<Product> SearchAndFilterCombined(
        string? keyword = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStockOnly = null)
    {
        IEnumerable<Product> query = _products;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.Name.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => string.Equals(p.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStockOnly.HasValue && inStockOnly.Value)
        {
            query = query.Where(p => p.IsInStock);
        }

        return query.ToList();
    }

    /// <summary>
    /// Returns unique list of categories across all products.
    /// </summary>
    public IEnumerable<string> GetDistinctCategories()
    {
        return _products.Select(p => p.Category).Distinct().OrderBy(c => c);
    }

    /// <summary>
    /// Returns overall catalog inventory statistics.
    /// </summary>
    public (int TotalCount, int InStockCount, int OutOfStockCount, decimal TotalInventoryValue, decimal OverallAveragePrice) GetOverallInventorySummary()
    {
        if (_products.Count == 0)
            return (0, 0, 0, 0m, 0m);

        var total = _products.Count;
        var inStock = _products.Count(p => p.IsInStock);
        var outOfStock = total - inStock;
        var totalValue = _products.Sum(p => p.Price * p.StockQuantity);
        var avgPrice = _products.Average(p => p.Price);

        return (total, inStock, outOfStock, totalValue, avgPrice);
    }
}
