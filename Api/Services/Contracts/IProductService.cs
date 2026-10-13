using Api.Entities.Models;
using Api.Shared.DataTransferObject;

namespace Api.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken);
    Task<ProductDto> GetProductAsync(int id, CancellationToken cancellationToken);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken);
    Task<(UpdateProductDto productToPatch, Product product)> PartialUpdateProductAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesForPatchAsync(UpdateProductDto updateProduct, Product product, CancellationToken cancellationToken);
    Task UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken);
    Task DeleteProductAsync(int id, CancellationToken cancellationToken);
}
