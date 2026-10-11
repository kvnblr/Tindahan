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
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto, CancellationToken cancellationToken)
    {
        if (productDto == null) return BadRequest("product is null");
        var product = await service.CreateProductAsync(productDto, cancellationToken);
        return CreatedAtRoute(nameof(GetProductById), new { id = product.Id }, product);
    }
}
