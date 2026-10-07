using Api.Entities.ExceptionModel;
using Api.Persistence;
using Api.Services.Contracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shared.DataTransferObject;

namespace Api.Services;

public class ProductService(ApiContext context, IMapper mapper) : IProductService
{
    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync()
            ?? throw new ProductNotFoundException(id);

        var productDto = mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = await context.Products
                .AsNoTracking()
                .ToListAsync();
        var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);
        return productsDto;
    }
}
