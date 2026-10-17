using System.ComponentModel.DataAnnotations;
using Api.Validations;

namespace Api.Shared.DataTransferObject;

[PriceRangeAttribute(nameof(MinimumPrice), nameof(MaxPrice), ErrorMessage = "Minimum price cannot exceed maximum price.")]
public sealed record RequestParametersDto
{
    [Range(1, 50, ErrorMessage = "Page size must be between 1 - 50.")]
    public int PageSize { get; init; } = 5;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    public int PageNumber { get; init; } = 1;

    public string SearchTerm { get; init; } = string.Empty;

    [AllowedSortFields("id", "name", "price", ErrorMessage = "Sort must be name, price, or id.")]
    public string SortBy { get; init; } = "id";

    public bool SortDescending { get; init; } = false;

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Minimum price cannot be negative.")]
    public decimal? MinimumPrice { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Maximum price cannot be negative.")]
    public decimal? MaxPrice { get; init; }
}

