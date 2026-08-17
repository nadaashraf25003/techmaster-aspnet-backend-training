using ProductCatalog.Models;
using ProductCatalog.Services;

namespace ProductCatalog.UI;

public class ConsoleMenu
{
    private readonly ProductQueryService _queryService;

    public ConsoleMenu(ProductQueryService queryService)
    {
        _queryService = queryService;
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            DisplayMenu();
            Console.Write("Enter your choice (1-11): ");
            var raw = Console.ReadLine();
            if (raw == null)
            {
                break;
            }
            var input = raw.Trim();

            Console.WriteLine();

            switch (input)
            {
                case "1":
                    HandleViewAvailableProducts();
                    break;
                case "2":
                    HandleFilterByCategory();
                    break;
                case "3":
                    HandleFilterByPriceRange();
                    break;
                case "4":
                    HandleSearchByName();
                    break;
                case "5":
                    HandleSortByPrice();
                    break;
                case "6":
                    HandleGroupByCategory();
                    break;
                case "7":
                    HandleStockValueReports();
                    break;
                case "8":
                    HandleLowStockProducts();
                    break;
                case "9":
                    HandleSupplierReport();
                    break;
                case "10":
                    HandlePaginationDemo();
                    break;
                case "11":
                    running = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Exiting Product Catalog LINQ System. Goodbye!\n");
                    Console.ResetColor();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option! Please enter a number between 1 and 11.");
                    Console.ResetColor();
                    break;
            }

            if (running)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press Enter to continue...");
                Console.ResetColor();

                if (Console.IsInputRedirected)
                {
                    Console.ReadLine();
                }
                else
                {
                    Console.ReadKey(intercept: true);
                    try { Console.Clear(); } catch { /* Ignore clear in environments where unsupported */ }
                }
            }
        }
    }

    private void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.WriteLine("   Product Catalog LINQ System");
        Console.WriteLine("==========================================");
        Console.ResetColor();
        Console.WriteLine(" 1. View Available Products (In Stock)");
        Console.WriteLine(" 2. Filter by Category");
        Console.WriteLine(" 3. Filter by Price Range");
        Console.WriteLine(" 4. Search by Name");
        Console.WriteLine(" 5. Sort by Price");
        Console.WriteLine(" 6. Group by Category");
        Console.WriteLine(" 7. Stock Value Reports & Category Stats");
        Console.WriteLine(" 8. Low Stock Products");
        Console.WriteLine(" 9. Supplier Report");
        Console.WriteLine("10. Pagination Demo");
        Console.WriteLine("11. Exit");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.ResetColor();
    }

    private void HandleViewAvailableProducts()
    {
        ConsoleFormatter.PrintHeader("Query: View Available Products (Stock > 0)");
        var available = _queryService.GetAvailableProducts();
        ConsoleFormatter.PrintProductTable(available, "No available products in stock.");
    }

    private void HandleFilterByCategory()
    {
        ConsoleFormatter.PrintHeader("Query: Filter by Category");
        var categories = _queryService.GetDistinctCategories().ToList();
        
        Console.WriteLine("Available Categories:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {categories[i]}");
        }
        Console.WriteLine();

        Console.Write("Enter Category name (or number): ");
        var input = Console.ReadLine()?.Trim();

        string selectedCategory = input ?? string.Empty;
        if (int.TryParse(input, out int index) && index >= 1 && index <= categories.Count)
        {
            selectedCategory = categories[index - 1];
        }

        var results = _queryService.FilterByCategory(selectedCategory);
        ConsoleFormatter.PrintSubHeader($"Results for Category: \"{selectedCategory}\"");
        ConsoleFormatter.PrintProductTable(results, $"No products found for category '{selectedCategory}'.");
    }

    private void HandleFilterByPriceRange()
    {
        ConsoleFormatter.PrintHeader("Query: Filter by Price Range");

        decimal minPrice = PromptDecimal("Enter Minimum Price ($): ", 0m);
        decimal maxPrice = PromptDecimal("Enter Maximum Price ($): ", 999999m);

        if (minPrice > maxPrice)
        {
            (minPrice, maxPrice) = (maxPrice, minPrice);
            Console.WriteLine($"[Note] Swapped prices: Min=${minPrice:F2}, Max=${maxPrice:F2}");
        }

        var results = _queryService.FilterByPriceRange(minPrice, maxPrice);
        ConsoleFormatter.PrintSubHeader($"Products between ${minPrice:F2} and ${maxPrice:F2}");
        ConsoleFormatter.PrintProductTable(results, $"No products found in the price range ${minPrice:F2} - ${maxPrice:F2}.");
    }

    private void HandleSearchByName()
    {
        ConsoleFormatter.PrintHeader("Query: Search by Name");
        Console.Write("Enter product name or keyword: ");
        var keyword = Console.ReadLine()?.Trim() ?? string.Empty;

        var results = _queryService.SearchByName(keyword);
        ConsoleFormatter.PrintSubHeader($"Search results for: \"{keyword}\"");
        ConsoleFormatter.PrintProductTable(results, $"No products matching '{keyword}' were found.");
    }

    private void HandleSortByPrice()
    {
        ConsoleFormatter.PrintHeader("Query: Sort by Price");
        Console.WriteLine("1. Low to High (Ascending)");
        Console.WriteLine("2. High to Low (Descending)");
        Console.Write("Choose sorting order (1 or 2): ");
        var choice = Console.ReadLine()?.Trim();

        bool ascending = choice != "2";
        var results = _queryService.SortByPrice(ascending);

        ConsoleFormatter.PrintSubHeader(ascending ? "Products Sorted by Price (Ascending - Low to High)" : "Products Sorted by Price (Descending - High to Low)");
        ConsoleFormatter.PrintProductTable(results);
    }

    private void HandleGroupByCategory()
    {
        ConsoleFormatter.PrintHeader("Query: Group by Category");
        var groups = _queryService.GroupByCategory();
        ConsoleFormatter.PrintGroupedByCategory(groups);
    }

    private void HandleStockValueReports()
    {
        ConsoleFormatter.PrintHeader("Query: Stock Value Reports & Category Statistics");

        var (total, inStock, outOfStock, totalValue, avgPrice) = _queryService.GetOverallInventorySummary();
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"=== Overall Catalog Summary ===");
        Console.WriteLine($"Total Products Count : {total}");
        Console.WriteLine($"In-Stock Products    : {inStock}");
        Console.WriteLine($"Out-of-Stock Products: {outOfStock}");
        Console.WriteLine($"Average Product Price: ${avgPrice:F2}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Inventory Value: ${totalValue:N2}");
        Console.ResetColor();
        Console.WriteLine();

        ConsoleFormatter.PrintSubHeader("Category Breakdown Statistics (Query 17)");
        var stats = _queryService.GetCategoryStatistics();
        ConsoleFormatter.PrintCategoryStatsTable(stats);

        ConsoleFormatter.PrintSubHeader("Products Above Average Price (Query 18)");
        var (averagePrice, aboveAvgProducts) = _queryService.GetProductsAboveAveragePrice();
        Console.WriteLine($"Average Catalog Price: ${averagePrice:F2}");
        ConsoleFormatter.PrintProductTable(aboveAvgProducts);

        ConsoleFormatter.PrintSubHeader("Recently Added Products (Last 60 Days) (Query 16)");
        var recentProducts = _queryService.GetRecentlyAddedProducts(60);
        ConsoleFormatter.PrintProductTable(recentProducts);
    }

    private void HandleLowStockProducts()
    {
        ConsoleFormatter.PrintHeader("Query: Low Stock Products");
        int threshold = PromptInt("Enter low stock threshold [Default = 5]: ", 5);

        var results = _queryService.GetLowStockProducts(threshold);
        ConsoleFormatter.PrintSubHeader($"Products with Stock Quantity <= {threshold}");
        ConsoleFormatter.PrintProductTable(results, $"No products found with stock quantity <= {threshold}.");
    }

    private void HandleSupplierReport()
    {
        ConsoleFormatter.PrintHeader("Query: Supplier Report (Query 15)");
        var reports = _queryService.GetSupplierReport();
        ConsoleFormatter.PrintSupplierReportTable(reports);
    }

    private void HandlePaginationDemo()
    {
        ConsoleFormatter.PrintHeader("Query: Pagination Simulation (Query 20)");

        int pageSize = PromptInt("Enter page size (items per page) [Default = 5]: ", 5);
        if (pageSize < 1) pageSize = 5;

        int currentPage = 1;
        bool paging = true;

        while (paging)
        {
            var pagedResult = _queryService.GetPagedProducts(currentPage, pageSize);

            ConsoleFormatter.PrintSubHeader($"Page {pagedResult.PageNumber} of {pagedResult.TotalPages} (Total Items: {pagedResult.TotalItems})");
            ConsoleFormatter.PrintProductTable(pagedResult.Items);

            Console.WriteLine("Pagination Controls:");
            Console.WriteLine("[N] Next Page  |  [P] Previous Page  |  [G] Go to Page #  |  [Q] Quit Pagination");
            Console.Write("Enter action: ");
            var rawCmd = Console.ReadLine();
            if (rawCmd == null)
            {
                break;
            }
            var cmd = rawCmd.Trim().ToUpperInvariant();

            switch (cmd)
            {
                case "N":
                    if (pagedResult.HasNextPage)
                        currentPage++;
                    else
                        PrintWarning("Already at the last page.");
                    break;
                case "P":
                    if (pagedResult.HasPreviousPage)
                        currentPage--;
                    else
                        PrintWarning("Already at the first page.");
                    break;
                case "G":
                    int targetPage = PromptInt($"Enter page number (1-{pagedResult.TotalPages}): ", currentPage);
                    if (targetPage >= 1 && targetPage <= pagedResult.TotalPages)
                        currentPage = targetPage;
                    else
                        PrintWarning($"Invalid page number. Must be between 1 and {pagedResult.TotalPages}.");
                    break;
                case "Q":
                    paging = false;
                    break;
                default:
                    PrintWarning("Unknown command. Use N, P, G, or Q.");
                    break;
            }
            Console.WriteLine();
        }
    }

    private static decimal PromptDecimal(string prompt, decimal defaultValue)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        if (decimal.TryParse(input, out decimal value))
            return value;

        Console.WriteLine($"Invalid number format. Using default: {defaultValue}");
        return defaultValue;
    }

    private static int PromptInt(string prompt, int defaultValue)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        if (int.TryParse(input, out int value))
            return value;

        Console.WriteLine($"Invalid number format. Using default: {defaultValue}");
        return defaultValue;
    }

    private static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[!] {message}");
        Console.ResetColor();
    }
}
