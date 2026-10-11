using Api.Entities.ExceptionModel;
using Api.Entities.Models;
using Api.Persistence;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ProductService(ApiContext context, IMapper mapper) : IProductService
{
    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken)
    {
        var productExist = await context.Products.AnyAsync(p => p.Name == createProductDto.Name, cancellationToken);
        if (productExist)
            throw new DuplicateProductException();

        var product = mapper.Map<Product>(createProductDto);
        context.Products.Add(product);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException();
        }

        var productDto = mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<ProductDto> GetProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ProductNotFoundException(id);

        var productDto = mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await context.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);
        return productsDto;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627);
    }
}
