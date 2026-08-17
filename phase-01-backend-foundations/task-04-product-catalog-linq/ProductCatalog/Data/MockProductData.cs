using ProductCatalog.Models;

namespace ProductCatalog.Data;

public static class MockProductData
{
    public static List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Laptop Pro 14",
                Category = "Electronics",
                Price = 45000m,
                StockQuantity = 5,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 1, 10)
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Category = "Electronics",
                Price = 650m,
                StockQuantity = 50,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 2, 1)
            },
            new Product
            {
                Id = 3,
                Name = "Office Chair",
                Category = "Furniture",
                Price = 3500m,
                StockQuantity = 10,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2025, 12, 15)
            },
            new Product
            {
                Id = 4,
                Name = "Standing Desk",
                Category = "Furniture",
                Price = 8000m,
                StockQuantity = 3,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2026, 3, 5)
            },
            new Product
            {
                Id = 5,
                Name = "Notebook Pack",
                Category = "Stationery",
                Price = 120m,
                StockQuantity = 100,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 1, 20)
            },
            new Product
            {
                Id = 6,
                Name = "Pen Set",
                Category = "Stationery",
                Price = 75m,
                StockQuantity = 200,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 1, 25)
            },
            new Product
            {
                Id = 7,
                Name = "Gaming Keyboard",
                Category = "Electronics",
                Price = 2500m,
                StockQuantity = 7,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 2, 12)
            },
            new Product
            {
                Id = 8,
                Name = "Monitor 27 inch",
                Category = "Electronics",
                Price = 9000m,
                StockQuantity = 4,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 2, 20)
            },
            new Product
            {
                Id = 9,
                Name = "Desk Lamp",
                Category = "Furniture",
                Price = 650m,
                StockQuantity = 0,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2025, 11, 1)
            },
            new Product
            {
                Id = 10,
                Name = "Backpack",
                Category = "Accessories",
                Price = 1200m,
                StockQuantity = 15,
                Supplier = "BagSupplier",
                CreatedAt = new DateTime(2026, 3, 10)
            },
            new Product
            {
                Id = 11,
                Name = "USB-C Hub",
                Category = "Electronics",
                Price = 1250m,
                StockQuantity = 12,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 4, 1)
            },
            new Product
            {
                Id = 12,
                Name = "Whiteboard Markers",
                Category = "Stationery",
                Price = 95m,
                StockQuantity = 80,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 2, 15)
            },
            new Product
            {
                Id = 13,
                Name = "Ergonomic Mouse Pad",
                Category = "Accessories",
                Price = 350m,
                StockQuantity = 25,
                Supplier = "BagSupplier",
                CreatedAt = new DateTime(2026, 5, 1)
            },
            new Product
            {
                Id = 14,
                Name = "Meeting Table",
                Category = "Furniture",
                Price = 12500m,
                StockQuantity = 2,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2025, 10, 20)
            },
            new Product
            {
                Id = 15,
                Name = "HD Webcam",
                Category = "Electronics",
                Price = 1800m,
                StockQuantity = 6,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 4, 17)
            },
            new Product
            {
                Id = 16,
                Name = "Printer Paper Box",
                Category = "Stationery",
                Price = 450m,
                StockQuantity = 30,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 2, 28)
            },
            new Product
            {
                Id = 17,
                Name = "Laptop Stand",
                Category = "Accessories",
                Price = 950m,
                StockQuantity = 9,
                Supplier = "BagSupplier",
                CreatedAt = new DateTime(2026, 3, 30)
            },
            new Product
            {
                Id = 18,
                Name = "Network Cable 5m",
                Category = "Electronics",
                Price = 150m,
                StockQuantity = 60,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 1, 5)
            },
            new Product
            {
                Id = 19,
                Name = "Storage Cabinet",
                Category = "Furniture",
                Price = 6000m,
                StockQuantity = 1,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2025, 9, 10)
            },
            new Product
            {
                Id = 20,
                Name = "Sticky Notes",
                Category = "Stationery",
                Price = 60m,
                StockQuantity = 0,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 5, 10)
            },
            new Product
            {
                Id = 21,
                Name = "Noise Cancelling Headset",
                Category = "Electronics",
                Price = 5200m,
                StockQuantity = 4,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 3, 22)
            },
            new Product
            {
                Id = 22,
                Name = "Desk Organizer",
                Category = "Accessories",
                Price = 300m,
                StockQuantity = 40,
                Supplier = "BagSupplier",
                CreatedAt = new DateTime(2026, 6, 1)
            },
            new Product
            {
                Id = 23,
                Name = "Projector",
                Category = "Electronics",
                Price = 22000m,
                StockQuantity = 2,
                Supplier = "TechSupplier",
                CreatedAt = new DateTime(2026, 4, 28)
            },
            new Product
            {
                Id = 24,
                Name = "Office Sofa",
                Category = "Furniture",
                Price = 15500m,
                StockQuantity = 1,
                Supplier = "HomeSupplier",
                CreatedAt = new DateTime(2025, 8, 18)
            },
            new Product
            {
                Id = 25,
                Name = "Calculator",
                Category = "Stationery",
                Price = 250m,
                StockQuantity = 35,
                Supplier = "PaperSupplier",
                CreatedAt = new DateTime(2026, 1, 12)
            }
        };
    }
}
