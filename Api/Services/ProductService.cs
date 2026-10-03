using Api.Persistence;
using Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.DataTransferObject;

namespace Api.Services;

public class ProductService(ApiContext context) : IProductService
{
    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync();
        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
        return productDto;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = await context.Products.AsNoTracking().ToListAsync();
        var productsDto = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock
        }).ToList();
        return productsDto;
    }
}
