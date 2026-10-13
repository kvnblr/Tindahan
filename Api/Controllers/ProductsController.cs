using Api.Entities.ExceptionModel;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
        IProductService service
        ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        var products = await service.GetProductsAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}", Name = nameof(GetProductById))]
    public async Task<IActionResult> GetProductById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var product = await service.GetProductAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto, CancellationToken cancellationToken)
    {
        if (createProductDto == null) return BadRequest("product is null");
        var product = await service.CreateProductAsync(createProductDto, cancellationToken);
        return CreatedAtRoute(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        await service.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto, CancellationToken cancellationToken)
    {
        if (updateProductDto == null) return BadRequest("product is null");
        await service.UpdateProductAsync(id, updateProductDto, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PartialUpdateProduct([FromRoute] int id, [FromBody] JsonPatchDocument<UpdateProductDto> patchDocument, CancellationToken cancellationToken)
    {
        if (patchDocument is null)
            throw new PatchBadRequestException("Patch document is required.");

        var (productToPatch, product) = await service.PartialUpdateProductAsync(id, cancellationToken);
        var patchErrors = new List<string>();
        patchDocument.ApplyTo(productToPatch, error => patchErrors.Add(error.ErrorMessage));

        if (patchErrors.Count > 0)
            throw new PatchBadRequestException($"Invalid patch operation: {string.Join("; ", patchErrors)}");

        if (!TryValidateModel(productToPatch))
            return UnprocessableEntity();

        await service.SaveChangesForPatchAsync(productToPatch, product, cancellationToken);
        return NoContent();
    }
}
