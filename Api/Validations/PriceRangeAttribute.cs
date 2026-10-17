using System.ComponentModel.DataAnnotations;
namespace Api.Validations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PriceRangeAttribute(string minimumProperty, string maximumProperty) : ValidationAttribute
{
    private readonly string _minimumProperty = minimumProperty;
    private readonly string _maximumProperty = maximumProperty;

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var type = value.GetType();

        var minimumProperty = type.GetProperty(_minimumProperty);
        var maximumProperty = type.GetProperty(_maximumProperty);

        if (minimumProperty is null || maximumProperty is null)
        {
            throw new InvalidOperationException(
                $"Properties '{_minimumProperty}' and/or " +
                $"'{_maximumProperty}' were not found on {type.Name}.");
        }

        var minimum = minimumProperty.GetValue(value) as decimal?;
        var maximum = maximumProperty.GetValue(value) as decimal?;

        if (minimum.HasValue &&
            maximum.HasValue &&
            minimum.Value > maximum.Value)
        {
            return new ValidationResult(
                ErrorMessage ??
                "Minimum price cannot exceed maximum price.",
                [_minimumProperty, _maximumProperty]);
        }

        return ValidationResult.Success;
    }
}
