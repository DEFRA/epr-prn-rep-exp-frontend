using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations;

/// <summary>
/// Defines a validation attribute that validates that a collection is not empty.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MustNotBeEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IList list)
        {
            var valid = list.Count > 0;
            if (valid)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(WastePermitExemptions.error_message_no_option_selected,
                [validationContext?.MemberName!]);
        }

        return ValidationResult.Success;
    }
}