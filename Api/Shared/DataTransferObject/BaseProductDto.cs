using System.ComponentModel.DataAnnotations;

namespace Api.Shared.DataTransferObject;

public abstract record BaseProductDto
{
    [Required]
    [MaxLength(20, ErrorMessage = "Maximum length of NAME must be 20 characters")]
    public string Name { get; init; } = string.Empty;
    [Required]
    [Range(0.01, 1000000, ErrorMessage = "The {0} field must be between {1} and {2}.")]
    public decimal Price { get; init; }
    [Range(0, int.MaxValue, ErrorMessage = "The {0} field must be between {1} and {2}.")]
    public int Stock { get; init; }
}
