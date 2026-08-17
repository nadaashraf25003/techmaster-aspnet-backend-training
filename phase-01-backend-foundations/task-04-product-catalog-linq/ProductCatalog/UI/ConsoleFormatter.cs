using ProductCatalog.Models;

namespace ProductCatalog.UI;

public static class ConsoleFormatter
{
    public static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 88));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 88));
        Console.ResetColor();
    }

    public static void PrintSubHeader(string subtitle)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"--- {subtitle} ---");
        Console.ResetColor();
    }

    public static void PrintProductTable(IEnumerable<Product> products, string? emptyMessage = "No products found matching the criteria.")
    {
        var productList = products.ToList();
        if (productList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  [!] {emptyMessage}\n");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 106));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{"ID",-4} | {"Product Name",-38} | {"Category",-14} | {"Price",-10} | {"Stock",-7} | {"Supplier",-14} | {"Created",-10}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 106));
        Console.ResetColor();

        foreach (var p in productList)
        {
            Console.Write($"{p.Id,-4} | ");
            Console.Write($"{Truncate(p.Name, 38),-38} | ");
            Console.Write($"{p.Category,-14} | ");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"${p.Price,8:F2} ");
            Console.ResetColor();
            Console.Write("| ");

            if (p.StockQuantity == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{"0 (Out)",-7}");
            }
            else if (p.StockQuantity <= 5)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{p.StockQuantity + " (Low)",-7}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{p.StockQuantity,-7}");
            }
            Console.ResetColor();
            Console.Write(" | ");

            Console.Write($"{p.Supplier,-14} | ");
            Console.WriteLine($"{p.CreatedAt:yyyy-MM-dd}");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 106));
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"Total Records: {productList.Count} | Total Inventory Value: ${productList.Sum(p => p.TotalStockValue):N2}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintCategoryStatsTable(IEnumerable<CategoryStats> stats)
    {
        var list = stats.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("No category statistics available.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 92));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{"Category",-18} | {"Count",-7} | {"Avg Price",-12} | {"Min Price",-12} | {"Max Price",-12} | {"Stock Value",-14}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 92));
        Console.ResetColor();

        foreach (var s in list)
        {
            Console.Write($"{s.Category,-18} | ");
            Console.Write($"{s.ProductCount,-7} | ");
            Console.Write($"${s.AveragePrice,10:F2} | ");
            Console.Write($"${s.MinPrice,10:F2} | ");
            Console.Write($"${s.MaxPrice,10:F2} | ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"${s.TotalStockValue,12:N2}");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 92));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"TOTALS",-18} | {list.Sum(x => x.ProductCount),-7} | {"-",-12} | {"-",-12} | {"-",-12} | ${list.Sum(x => x.TotalStockValue),12:N2}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintSupplierReportTable(IEnumerable<SupplierReport> reports)
    {
        var list = reports.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("No supplier records available.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 78));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{"Supplier Name",-20} | {"Total Products",-16} | {"Average Price",-15} | {"Total Stock Value",-18}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 78));
        Console.ResetColor();

        foreach (var r in list)
        {
            Console.Write($"{r.SupplierName,-20} | ");
            Console.Write($"{r.TotalProducts,-16} | ");
            Console.Write($"${r.AveragePrice,13:F2} | ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"${r.TotalStockValue,16:N2}");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 78));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"TOTALS",-20} | {list.Sum(x => x.TotalProducts),-16} | {"-",-15} | ${list.Sum(x => x.TotalStockValue),16:N2}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintGroupedByCategory(IEnumerable<IGrouping<string, Product>> groups)
    {
        foreach (var group in groups)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n[+] Category: {group.Key.ToUpperInvariant()} ({group.Count()} products, Total Value: ${group.Sum(p => p.TotalStockValue):N2})");
            Console.ResetColor();

            PrintProductTable(group);
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}
