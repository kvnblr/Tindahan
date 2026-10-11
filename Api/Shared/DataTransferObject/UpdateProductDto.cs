using System.ComponentModel.DataAnnotations;

namespace Api.Shared.DataTransferObject;

public record UpdateProductDto
{
    [Required]
    [MaxLength(20, ErrorMessage = "Maximum length of NAME must be 20 characters")]
    public string? Name { get; init; }
    [Required]
    [Range(0.01, 1000000)]
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
