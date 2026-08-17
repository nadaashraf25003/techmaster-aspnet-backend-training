# Task 04 - Product Catalog LINQ System

A high-performance, console-based Product Catalog Query and Reporting System built with C# (.NET 10.0) following Clean Architecture principles and demonstrating core-to-advanced LINQ querying, deferred execution, and statistical projections.

---

## Architecture & Design Patterns

The system adheres to a strict 3-Layer separation of concerns:

```
task-04-product-catalog-linq/
│
├── Program.cs                             ← Application bootstrapper and CLI dispatcher
├── README.md                              ← System architecture & LINQ query explanations
├── task-04-product-catalog-linq.csproj    ← .NET 10.0 project configuration
│
└── ProductCatalog/
    ├── Models/
    │   ├── Product.cs                     ← Core domain model with computed stock value & in-stock flags
    │   ├── CategoryStats.cs               ← Projection DTO for category aggregations (Query 17)
    │   ├── SupplierReport.cs              ← Projection DTO for supplier analytics (Query 15)
    │   └── PagedResult.cs                 ← Generic pagination metadata wrapper (Query 20)
    │
    ├── Data/
    │   └── MockProductData.cs             ← Rich seeded catalog across 6+ categories & 7 suppliers
    │
    ├── Services/
    │   └── ProductQueryService.cs         ← Encapsulates all 20 LINQ queries with clean interfaces
    │
    └── UI/
        ├── ConsoleMenu.cs                 ← Interactive 11-option console menu loop & validation
        ├── ConsoleFormatter.cs            ← Tabular ASCII grid rendering, badges & currency formatting
        └── QueryShowcase.cs               ← Automated CLI runner executing all 20 queries
```

---

## Interactive Menu Options

```
====== Product Catalog LINQ System ======
 1. View Available Products (In Stock)
 2. Filter by Category
 3. Filter by Price Range
 4. Search by Name
 5. Sort by Price (Ascending / Descending)
 6. Group by Category
 7. Stock Value Reports & Category Stats
 8. Low Stock Products
 9. Supplier Report
10. Pagination Demo
11. Exit
==========================================
```

---

## In-Depth LINQ Query Explanations

### 1. Query 15 - Supplier Report (`GroupBy` + `Select`)
- **Objective**: Group products by supplier and calculate total product count, total inventory stock value, and average product price per supplier.
- **LINQ Concepts**: `GroupBy`, `Select`, `Sum`, `Average`, `OrderByDescending`.
- **Implementation**:
```csharp
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
```
- **Why it matters**: Demonstrates projection of grouped collections into strongly-typed reporting DTOs with safe aggregate calculations.

---

### 2. Query 16 - Recently Added Products (`Where` + `DateTime`)
- **Objective**: Filter products created within the last 60 days relative to a reference date.
- **LINQ Concepts**: `Where`, `DateTime` date arithmetic, `OrderByDescending`.
- **Implementation**:
```csharp
public IEnumerable<Product> GetRecentlyAddedProducts(int days = 60, DateTime? referenceDate = null)
{
    var targetDate = (referenceDate ?? DateTime.Today).AddDays(-days);
    return _products
        .Where(p => p.CreatedAt >= targetDate)
        .OrderByDescending(p => p.CreatedAt);
}
```
- **Why it matters**: Essential for "What's New" endpoints and dynamic time-window filtering.

---

### 3. Query 17 - Category Statistics (`GroupBy` + `Select` with Multi-Aggregates)
- **Objective**: For each category, compute product count, average price, minimum price, maximum price, and total stock valuation while safely handling empty groups.
- **LINQ Concepts**: `GroupBy`, `Select`, `Average`, `Min`, `Max`, `Sum`, `Any`.
- **Implementation**:
```csharp
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
```
- **Why it matters**: Computes complex analytical rollups in a single expressive LINQ statement.

---

### 4. Query 18 - Products Above Average Price (`Average` + `Where`)
- **Objective**: Calculate the catalog-wide average price and retrieve all items priced strictly higher than the average.
- **LINQ Concepts**: `Average`, `Where`, `OrderByDescending`.
- **Implementation**:
```csharp
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
```
- **Why it matters**: Demonstrates two-step LINQ composition: aggregating first to compute a threshold, then applying deferred filtering.

---

### 5. Query 19 - Search + Filter Combined (`Where` Chaining)
- **Objective**: Dynamically combine keyword search, category match, price range boundaries, and in-stock criteria.
- **LINQ Concepts**: Deferred Execution, Fluent `Where` Chaining.
- **Implementation**:
```csharp
public IEnumerable<Product> SearchAndFilterCombined(
    string? keyword = null,
    string? category = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    bool? inStockOnly = null)
{
    IEnumerable<Product> query = _products;

    if (!string.IsNullOrWhiteSpace(keyword))
        query = query.Where(p => p.Name.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
        query = query.Where(p => string.Equals(p.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));

    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);

    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    if (inStockOnly.HasValue && inStockOnly.Value)
        query = query.Where(p => p.IsInStock);

    return query.ToList();
}
```
- **Why it matters**: Matches real-world backend API query pipelines with optional query parameters.

---

### 6. Query 20 - Pagination Simulation (`Skip` + `Take`)
- **Objective**: Efficiently partition products into pages based on 1-based page number and page size.
- **LINQ Concepts**: `Skip`, `Take`, parameter validation.
- **Implementation**:
```csharp
public PagedResult<Product> GetPagedProducts(int pageNumber, int pageSize)
{
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 5;

    var totalItems = _products.Count;
    var pagedItems = _products
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize);

    return new PagedResult<Product>(pagedItems, pageNumber, pageSize, totalItems);
}
```
- **Why it matters**: Foundation for scalable REST APIs, avoiding loading entire datasets into client memory.
