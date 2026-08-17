using ProductCatalog.Models;
using ProductCatalog.Services;

namespace ProductCatalog.UI;

public static class QueryShowcase
{
    public static void RunAll(ProductQueryService service)
    {
        ConsoleFormatter.PrintHeader("TASK 04: LINQ QUERIES SHOWCASE & VERIFICATION");

        // Query 1 / Menu 1: Available Products
        Console.WriteLine("\n>>> Query 01 / Menu 1: View Available Products (StockQuantity > 0)");
        var available = service.GetAvailableProducts();
        ConsoleFormatter.PrintProductTable(available.Take(5), "No available products.");
        Console.WriteLine($"[Note: Displaying first 5 of {available.Count()} available items]");

        // Query 2 / Menu 2: Filter by Category
        Console.WriteLine("\n>>> Query 02 / Menu 2: Filter by Category ('Electronics')");
        var electronics = service.FilterByCategory("Electronics");
        ConsoleFormatter.PrintProductTable(electronics);

        // Query 3 / Menu 3: Filter by Price Range
        Console.WriteLine("\n>>> Query 03 / Menu 3: Filter by Price Range ($50.00 - $200.00)");
        var priceRange = service.FilterByPriceRange(50m, 200m);
        ConsoleFormatter.PrintProductTable(priceRange);

        // Query 4 / Menu 4: Search by Name
        Console.WriteLine("\n>>> Query 04 / Menu 4: Search by Name ('Gaming')");
        var searchResults = service.SearchByName("Gaming");
        ConsoleFormatter.PrintProductTable(searchResults);

        // Query 5 / Menu 5: Sort by Price
        Console.WriteLine("\n>>> Query 05 / Menu 5: Sort by Price (Descending - Top 5 Most Expensive)");
        var sortedDesc = service.SortByPrice(ascending: false).Take(5);
        ConsoleFormatter.PrintProductTable(sortedDesc);

        // Query 6 / Menu 6: Group by Category
        Console.WriteLine("\n>>> Query 06 / Menu 6: Group by Category");
        var grouped = service.GroupByCategory();
        foreach (var group in grouped.Take(3))
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[*] Category Group: {group.Key} (Count: {group.Count()})");
            Console.ResetColor();
            ConsoleFormatter.PrintProductTable(group);
        }

        // Query 7 & 17 / Menu 7: Category Statistics
        Console.WriteLine("\n>>> Query 17 / Menu 7: Category Statistics (Count, Avg, Max, Min, Total Stock Value)");
        var categoryStats = service.GetCategoryStatistics();
        ConsoleFormatter.PrintCategoryStatsTable(categoryStats);

        // Query 8 / Menu 8: Low Stock Products
        Console.WriteLine("\n>>> Query 08 / Menu 8: Low Stock Products (StockQuantity <= 5)");
        var lowStock = service.GetLowStockProducts(5);
        ConsoleFormatter.PrintProductTable(lowStock);

        // Query 15 / Menu 9: Supplier Report
        Console.WriteLine("\n>>> Query 15 / Menu 9: Supplier Report (GroupBy + Select)");
        var supplierReports = service.GetSupplierReport();
        ConsoleFormatter.PrintSupplierReportTable(supplierReports);

        // Query 16: Recently Added Products (Last 60 Days)
        Console.WriteLine("\n>>> Query 16: Recently Added Products (CreatedAt >= Today - 60 Days)");
        var recentProducts = service.GetRecentlyAddedProducts(60);
        ConsoleFormatter.PrintProductTable(recentProducts);

        // Query 18: Products Above Average Price
        Console.WriteLine("\n>>> Query 18: Products Above Average Price");
        var (avgPrice, aboveAvg) = service.GetProductsAboveAveragePrice();
        Console.WriteLine($"Catalog Average Price: ${avgPrice:F2}");
        ConsoleFormatter.PrintProductTable(aboveAvg);

        // Query 19: Search + Combined Filtering
        Console.WriteLine("\n>>> Query 19: Search + Filter Combined (Category: 'Accessories', MaxPrice: $1000.00, InStock: true)");
        var combined = service.SearchAndFilterCombined(category: "Accessories", maxPrice: 1000m, inStockOnly: true);
        ConsoleFormatter.PrintProductTable(combined);

        // Query 20 / Menu 10: Pagination Simulation
        Console.WriteLine("\n>>> Query 20 / Menu 10: Pagination Simulation (Page 2, PageSize: 4)");
        var paged = service.GetPagedProducts(pageNumber: 2, pageSize: 4);
        Console.WriteLine($"Showing Page {paged.PageNumber} of {paged.TotalPages} (Total Items: {paged.TotalItems}, HasPrev: {paged.HasPreviousPage}, HasNext: {paged.HasNextPage})");
        ConsoleFormatter.PrintProductTable(paged.Items);

        ConsoleFormatter.PrintHeader("ALL 20 LINQ QUERIES EXECUTED SUCCESSFULLY");
    }
}
