using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
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
}
