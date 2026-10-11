using System.ComponentModel.DataAnnotations;

namespace Api.Shared.DataTransferObject;

public record ProductDto
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
