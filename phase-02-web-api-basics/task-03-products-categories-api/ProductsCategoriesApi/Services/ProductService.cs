using ProductsCategoriesApi.DTOs;
using ProductsCategoriesApi.Models;

namespace ProductsCategoriesApi.Services;

/// <summary>
/// Thread-safe in-memory implementation of product inventory management.
/// </summary>
public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private readonly ICategoryService _categoryService;
    private readonly Lock _lock = new();
    private int _nextId = 1;

    public ProductService(ICategoryService categoryService)
    {
        _categoryService = categoryService;
        SeedInitialProducts();
    }

    private void SeedInitialProducts()
    {
        var seeds = new List<Product>
        {
            // Category 1: Electronics (6 products)
            new()
            {
                ProductId = _nextId++,
                Name = "Pro Laptop 16 inch",
                CategoryId = 1,
                Price = 1499.99m,
                StockQuantity = 12,
                IsAvailable = true,
                SupplierName = "TechGlobal Supplies",
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Wireless Optical Mouse",
                CategoryId = 1,
                Price = 29.99m,
                StockQuantity = 4, // Low stock
                IsAvailable = true,
                SupplierName = "LogiGear Tech",
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Mechanical Gaming Keyboard",
                CategoryId = 1,
                Price = 89.99m,
                StockQuantity = 18,
                IsAvailable = true,
                SupplierName = "KeyCraft Hardware",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "4K UHD Monitor 27-inch",
                CategoryId = 1,
                Price = 349.99m,
                StockQuantity = 0, // Out of stock
                IsAvailable = false,
                SupplierName = "VisionTech Displays",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "7-in-1 USB-C Hub Multiport Adapter",
                CategoryId = 1,
                Price = 45.00m,
                StockQuantity = 25,
                IsAvailable = true,
                SupplierName = "AnkerPro Accessories",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Active Noise Cancelling Earbuds",
                CategoryId = 1,
                Price = 129.99m,
                StockQuantity = 8,
                IsAvailable = true,
                SupplierName = "SoundWave Audio",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },

            // Category 2: Furniture (3 products)
            new()
            {
                ProductId = _nextId++,
                Name = "Ergonomic Mesh Office Chair",
                CategoryId = 2,
                Price = 249.50m,
                StockQuantity = 3, // Low stock
                IsAvailable = true,
                SupplierName = "ErgoLiving Furniture",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Electric Height Adjustable Standing Desk",
                CategoryId = 2,
                Price = 499.00m,
                StockQuantity = 7,
                IsAvailable = true,
                SupplierName = "FlexiDesk Workspace",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Dimmable LED Architect Desk Lamp",
                CategoryId = 2,
                Price = 39.99m,
                StockQuantity = 15,
                IsAvailable = true,
                SupplierName = "BrightWorks Lighting",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },

            // Category 3: Stationery (4 products)
            new()
            {
                ProductId = _nextId++,
                Name = "Hardcover Dotted Grid Journal",
                CategoryId = 3,
                Price = 14.50m,
                StockQuantity = 50,
                IsAvailable = true,
                SupplierName = "PaperLoom Stationery",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Gel Ink Rollerball Pens (10-pack)",
                CategoryId = 3,
                Price = 9.99m,
                StockQuantity = 2, // Low stock
                IsAvailable = true,
                SupplierName = "WriteFine Instruments",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Chisel Tip Highlighter Set (6 Colors)",
                CategoryId = 3,
                Price = 6.50m,
                StockQuantity = 30,
                IsAvailable = true,
                SupplierName = "ColorGlow Office",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Premium A4 Multipurpose Copy Paper",
                CategoryId = 3,
                Price = 12.00m,
                StockQuantity = 0, // Out of stock
                IsAvailable = false,
                SupplierName = "PaperLoom Stationery",
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },

            // Category 4: Accessories (4 products)
            new()
            {
                ProductId = _nextId++,
                Name = "Waterproof Laptop Backpack 17-inch",
                CategoryId = 4,
                Price = 69.99m,
                StockQuantity = 14,
                IsAvailable = true,
                SupplierName = "UrbanShield Bags",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Ergonomic Memory Foam Mouse Pad",
                CategoryId = 4,
                Price = 16.99m,
                StockQuantity = 40,
                IsAvailable = true,
                SupplierName = "ComfortDesk Tech",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Shockproof Padded Laptop Sleeve 15.6",
                CategoryId = 4,
                Price = 22.50m,
                StockQuantity = 5, // Low stock boundary
                IsAvailable = true,
                SupplierName = "UrbanShield Bags",
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new()
            {
                ProductId = _nextId++,
                Name = "Compact Cable & Accessory Organizer Pouch",
                CategoryId = 4,
                Price = 18.00m,
                StockQuantity = 22,
                IsAvailable = true,
                SupplierName = "UrbanShield Bags",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        _products.AddRange(seeds);
    }

    public async Task<IEnumerable<ProductResponse>> GetFilteredProductsAsync(ProductFilterQuery query)
    {
        List<Product> snapshot;
        lock (_lock)
        {
            snapshot = _products.ToList();
        }

        var filtered = snapshot.AsEnumerable();

        // 1. Search term (Name or Supplier)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            filtered = filtered.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.SupplierName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Filter by CategoryId
        if (query.CategoryId.HasValue)
        {
            filtered = filtered.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        // 3. Filter by MinPrice
        if (query.MinPrice.HasValue)
        {
            filtered = filtered.Where(p => p.Price >= query.MinPrice.Value);
        }

        // 4. Filter by MaxPrice
        if (query.MaxPrice.HasValue)
        {
            filtered = filtered.Where(p => p.Price <= query.MaxPrice.Value);
        }

        // 5. Filter by IsAvailable
        if (query.IsAvailable.HasValue)
        {
            filtered = filtered.Where(p => p.IsAvailable == query.IsAvailable.Value);
        }

        // 6. Filter by IsLowStock
        if (query.IsLowStock.HasValue && query.IsLowStock.Value)
        {
            filtered = filtered.Where(p => p.StockQuantity <= query.LowStockThreshold);
        }

        var resultList = filtered.OrderBy(p => p.ProductId).ToList();
        var responseList = new List<ProductResponse>();

        foreach (var p in resultList)
        {
            var catName = await _categoryService.GetCategoryNameAsync(p.CategoryId) ?? "Unknown";
            responseList.Add(MapToDto(p, catName));
        }

        return responseList;
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        Product? product;
        lock (_lock)
        {
            product = _products.FirstOrDefault(p => p.ProductId == id);
        }

        if (product == null) return null;

        var catName = await _categoryService.GetCategoryNameAsync(product.CategoryId) ?? "Unknown";
        return MapToDto(product, catName);
    }

    public async Task<(bool Success, string? Error, ProductResponse? Data)> CreateProductAsync(CreateProductRequest request)
    {
        // Category validation
        var categoryExists = await _categoryService.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return (false, $"Category with ID {request.CategoryId} does not exist.", null);
        }

        var categoryName = await _categoryService.GetCategoryNameAsync(request.CategoryId) ?? "Unknown";

        lock (_lock)
        {
            var product = new Product
            {
                ProductId = _nextId++,
                Name = request.Name.Trim(),
                CategoryId = request.CategoryId,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsAvailable = request.IsAvailable,
                SupplierName = request.SupplierName.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _products.Add(product);
            return (true, null, MapToDto(product, categoryName));
        }
    }

    public async Task<(bool Success, string? Error, ProductResponse? Data)> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        // Category validation
        var categoryExists = await _categoryService.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
        {
            return (false, $"Category with ID {request.CategoryId} does not exist.", null);
        }

        var categoryName = await _categoryService.GetCategoryNameAsync(request.CategoryId) ?? "Unknown";

        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return (false, $"Product with ID {id} was not found.", null);
            }

            product.Name = request.Name.Trim();
            product.CategoryId = request.CategoryId;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.IsAvailable = request.IsAvailable;
            product.SupplierName = request.SupplierName.Trim();
            product.UpdatedAt = DateTime.UtcNow;

            return (true, null, MapToDto(product, categoryName));
        }
    }

    public async Task<(bool Success, string? Error, ProductResponse? Data)> UpdateStockAsync(int id, int stockQuantity)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return (false, $"Product with ID {id} was not found.", null);
            }

            product.StockQuantity = stockQuantity;
            if (stockQuantity == 0)
            {
                product.IsAvailable = false;
            }
            else if (stockQuantity > 0 && !product.IsAvailable)
            {
                product.IsAvailable = true;
            }

            product.UpdatedAt = DateTime.UtcNow;

            var catName = _categoryService.GetCategoryNameAsync(product.CategoryId).Result ?? "Unknown";
            return (true, null, MapToDto(product, catName));
        }
    }

    public Task<(bool Success, string? Error)> DeleteProductAsync(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return Task.FromResult<(bool, string?)>((false, $"Product with ID {id} was not found."));
            }

            _products.Remove(product);
            return Task.FromResult<(bool, string?)>((true, null));
        }
    }

    public async Task<IEnumerable<ProductResponse>> GetLowStockProductsAsync(int threshold = 5)
    {
        List<Product> lowStock;
        lock (_lock)
        {
            lowStock = _products.Where(p => p.StockQuantity <= threshold).OrderBy(p => p.StockQuantity).ToList();
        }

        var responseList = new List<ProductResponse>();
        foreach (var p in lowStock)
        {
            var catName = await _categoryService.GetCategoryNameAsync(p.CategoryId) ?? "Unknown";
            responseList.Add(MapToDto(p, catName));
        }

        return responseList;
    }

    public async Task<StockReportResponse> GetStockReportAsync()
    {
        List<Product> snapshot;
        lock (_lock)
        {
            snapshot = _products.ToList();
        }

        var categories = (await _categoryService.GetAllCategoriesAsync(includeInactive: true)).ToList();

        var totalValue = snapshot.Sum(p => p.Price * p.StockQuantity);
        var totalUnits = snapshot.Sum(p => p.StockQuantity);
        var lowStockList = snapshot.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 5).ToList();
        var outOfStockList = snapshot.Where(p => p.StockQuantity == 0 || !p.IsAvailable).ToList();

        var categorySummaries = new List<CategoryStockSummaryDto>();

        foreach (var cat in categories)
        {
            var catProducts = snapshot.Where(p => p.CategoryId == cat.CategoryId).ToList();
            categorySummaries.Add(new CategoryStockSummaryDto
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.Name,
                TotalProducts = catProducts.Count,
                TotalUnitsInStock = catProducts.Sum(p => p.StockQuantity),
                TotalCategoryValue = catProducts.Sum(p => p.Price * p.StockQuantity)
            });
        }

        var lowStockDtos = new List<ProductResponse>();
        foreach (var p in lowStockList)
        {
            var catName = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId)?.Name ?? "Unknown";
            lowStockDtos.Add(MapToDto(p, catName));
        }

        var outOfStockDtos = new List<ProductResponse>();
        foreach (var p in outOfStockList)
        {
            var catName = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId)?.Name ?? "Unknown";
            outOfStockDtos.Add(MapToDto(p, catName));
        }

        return new StockReportResponse
        {
            TotalStockValue = Math.Round(totalValue, 2),
            TotalProducts = snapshot.Count,
            TotalUnitsInStock = totalUnits,
            LowStockProductsCount = lowStockList.Count,
            OutOfStockProductsCount = outOfStockList.Count,
            StockValuePerCategory = categorySummaries,
            LowStockProducts = lowStockDtos,
            OutOfStockProducts = outOfStockDtos
        };
    }

    public Task<bool> HasProductsForCategoryAsync(int categoryId)
    {
        lock (_lock)
        {
            var hasProducts = _products.Any(p => p.CategoryId == categoryId);
            return Task.FromResult(hasProducts);
        }
    }

    public Task<int> GetProductCountForCategoryAsync(int categoryId)
    {
        lock (_lock)
        {
            var count = _products.Count(p => p.CategoryId == categoryId);
            return Task.FromResult(count);
        }
    }

    private static ProductResponse MapToDto(Product p, string categoryName)
    {
        return new ProductResponse
        {
            ProductId = p.ProductId,
            Name = p.Name,
            CategoryId = p.CategoryId,
            CategoryName = categoryName,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsAvailable = p.IsAvailable,
            SupplierName = p.SupplierName,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
