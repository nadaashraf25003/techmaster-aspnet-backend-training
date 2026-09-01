using RefactoredApi.DTOs;

namespace RefactoredApi.Services;

public interface IProductService
{
    IEnumerable<ProductResponse> GetAll();
    ProductResponse? GetById(int id);
    ProductResponse Create(CreateProductRequest request);
}
