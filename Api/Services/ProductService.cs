using Api.Entities.ExceptionModel;
using Api.Entities.Models;
using Api.Persistence;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Api.Shared.DataTransferObject.Common;
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
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        var productDto = mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(RequestParametersDto request, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(p => p.Name != null && p.Name.Contains(request.SearchTerm.Trim()));

        if (request.MinimumPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinimumPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        var sortBy = request.SortBy?.Trim().ToLowerInvariant() ?? "id";
        query = sortBy switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(p => p.Name).ThenBy(p => p.Id)
                : query.OrderBy(p => p.Name).ThenBy(p => p.Id),

            "price" => request.SortDescending
                ? query.OrderByDescending(p => p.Price).ThenBy(p => p.Id)
                : query.OrderBy(p => p.Price).ThenBy(p => p.Id),

            "id" => request.SortDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id),

            _ => throw new SortByArgumentException()
        };

        var offset = ((long)request.PageNumber - 1) * request.PageSize;
        if (offset > int.MaxValue)
            throw new OffsetArgumentOutOfRangeException();

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var products = await query
                .Skip((int)offset)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

        var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);

        return new PagedResult<ProductDto>
        {
            Items = productsDto,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var deleteRows = await context.Products
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

        if (deleteRows == 0)
            throw new ProductNotFoundException(id);
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken)
    {

        var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        mapper.Map(updateProductDto, product);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627);
    }

    public async Task<(UpdateProductDto productToPatch, Product product)> PartialUpdateProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        var productToPatch = mapper.Map<UpdateProductDto>(product);

        return (productToPatch, product);
    }

    public async Task SaveChangesForPatchAsync(UpdateProductDto updateProduct, Product product, CancellationToken cancellationToken)
    {
        mapper.Map(updateProduct, product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
