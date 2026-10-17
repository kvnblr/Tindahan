using System.ComponentModel.DataAnnotations;

namespace Api.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class AllowedSortFieldsAttribute(params string[] allowedfields) : ValidationAttribute
{
    private readonly HashSet<string> _allowedFields = new(allowedfields, StringComparer.OrdinalIgnoreCase);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string sortBy
                || string.IsNullOrWhiteSpace(sortBy)
                || !_allowedFields.Contains(sortBy.Trim()))
            return new ValidationResult(ErrorMessage ?? $"Sort must be one of: {string.Join(", ", _allowedFields)}");

        return ValidationResult.Success;
    }
}
