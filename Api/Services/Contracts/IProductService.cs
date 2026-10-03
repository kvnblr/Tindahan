using Shared.DataTransferObject;

namespace Api.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<ProductDto> GetProductAsync(int id);
}
