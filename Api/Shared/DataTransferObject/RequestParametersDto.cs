using System.ComponentModel.DataAnnotations;

namespace Api.Shared.DataTransferObject;

public sealed record RequestParametersDto : IValidatableObject
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "name", "price"
    };

    [Range(1, 50, ErrorMessage = "Page size must be between 1 - 50.")]
    public int PageSize { get; init; } = 5;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    public int PageNumber { get; init; } = 1;

    public string? SearchTerm { get; init; } = string.Empty;

    public string SortBy { get; init; } = "id";

    public bool SortDescending { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? MinimumPrice { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? MaxPrice { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SortBy) || !AllowedSortFields.Contains(SortBy.Trim()))
            yield return new ValidationResult("Sort must be name, price, or id", [nameof(SortBy)]);

        if (MinimumPrice.HasValue && MaxPrice.HasValue && MinimumPrice.Value > MaxPrice.Value)
            yield return new ValidationResult("Minimum price cannot exceeds maximum price.", [nameof(MinimumPrice), nameof(MaxPrice)]);
    }
}
