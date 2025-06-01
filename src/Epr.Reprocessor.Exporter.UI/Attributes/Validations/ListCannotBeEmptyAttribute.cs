using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations;

/// <summary>
/// Provides a validation attribute that checks that a list is not null or empty.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Property)]
public class ListCannotBeEmptyAttribute<T> : ValidationAttribute
{
    /// <inheritdoc />>.
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if ( value is null or List<T> { Count: 0 })
        {
            return new ValidationResult(ErrorMessage, new List<string>{validationContext?.MemberName!});
        }

        return ValidationResult.Success;
    }
}