using ProductCatalog.Data;
using ProductCatalog.Services;
using ProductCatalog.UI;

try { Console.Title = "Product Catalog LINQ System"; } catch { }
try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }

var seedProducts = MockProductData.GetSampleProducts();
var queryService = new ProductQueryService(seedProducts);

if (args.Length > 0 && (args[0].Equals("--demo", StringComparison.OrdinalIgnoreCase) ||
                       args[0].Equals("--test", StringComparison.OrdinalIgnoreCase) ||
                       args[0].Equals("--showcase", StringComparison.OrdinalIgnoreCase)))
{
    QueryShowcase.RunAll(queryService);
    return;
}

var menu = new ConsoleMenu(queryService);
menu.Run();
