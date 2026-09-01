using RefactoredApi.DTOs;
using RefactoredApi.Models;

namespace RefactoredApi.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<ProductResponse> GetAll()
    {
        lock (_lock)
        {
            return _products.Select(MapToResponse).ToList();
        }
    }

    public ProductResponse? GetById(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product == null ? null : MapToResponse(product);
        }
    }

    public ProductResponse Create(CreateProductRequest request)
    {
        lock (_lock)
        {
            var product = new Product
            {
                Id = _nextId++,
                Name = request.Name.Trim(),
                Price = request.Price,
                Stock = request.Stock
            };

            _products.Add(product);
            return MapToResponse(product);
        }
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}
