using Api.Entities.ExceptionModel;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Api.Shared.DataTransferObject.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
        IProductService productService,
        ILinkService linkService
        ) : ControllerBase
{
    [HttpGet(Name = nameof(GetAllProducts))]
    public async Task<IActionResult> GetAllProducts([FromQuery] RequestParametersDto request, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(request, cancellationToken);
        var productsResource = products.Items.Select(product => new ProductResourceDto
        {
            Id = product.Id,
            Name = product.Name ?? string.Empty,
            Price = product.Price,
            Stock = product.Stock,
            Links = linkService.CreateProductLinks(HttpContext, product.Id)
        }).ToList();

        var response = new PagedResult<ProductResourceDto>
        {
            Items = productsResource,
            PageNumber = products.PageNumber,
            PageSize = products.PageSize,
            TotalCount = products.TotalCount,
            TotalPages = products.TotalPages,
            Links = linkService.CreatePaginationLinks(HttpContext, request, products.TotalPages)
        };
        return Ok(response);
    }

    [HttpGet("{id:int}", Name = nameof(GetProduct))]
    public async Task<IActionResult> GetProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        var product = await productService.GetProductAsync(id, cancellationToken);
        var resource = new ProductResourceDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Links = linkService.CreateProductLinks(HttpContext, product.Id)
        };
        return Ok(resource);
    }

    [HttpPost(Name = nameof(CreateProduct))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto, CancellationToken cancellationToken)
    {
        if (createProductDto == null) return BadRequest("product is null");
        var product = await productService.CreateProductAsync(createProductDto, cancellationToken);
        var resource = new ProductResourceDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Links = linkService.CreateProductLinks(HttpContext, product.Id)
        };
        return CreatedAtRoute(nameof(GetProduct), new { id = product.Id }, resource);
    }

    [HttpDelete("{id:int}", Name = nameof(DeleteProduct))]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        await productService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}", Name = nameof(UpdateProduct))]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto, CancellationToken cancellationToken)
    {
        if (updateProductDto == null) return BadRequest("product is null");
        await productService.UpdateProductAsync(id, updateProductDto, cancellationToken);

        var product = await productService.GetProductAsync(id, cancellationToken);
        var resource = new ProductResourceDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Links = linkService.CreateProductLinks(HttpContext, product.Id)
        };
        return Ok(resource);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PartialUpdateProduct([FromRoute] int id, [FromBody] JsonPatchDocument<UpdateProductDto> patchDocument, CancellationToken cancellationToken)
    {
        if (patchDocument is null)
            throw new PatchBadRequestException("Patch document is required.");

        var (productToPatch, product) = await productService.PartialUpdateProductAsync(id, cancellationToken);
        var patchErrors = new List<string>();
        patchDocument.ApplyTo(productToPatch, error => patchErrors.Add(error.ErrorMessage));

        if (patchErrors.Count > 0)
            throw new PatchBadRequestException($"Invalid patch operation: {string.Join("; ", patchErrors)}");

        if (!TryValidateModel(productToPatch))
            return UnprocessableEntity();

        await productService.SaveChangesForPatchAsync(productToPatch, product, cancellationToken);
        return NoContent();
    }
}
