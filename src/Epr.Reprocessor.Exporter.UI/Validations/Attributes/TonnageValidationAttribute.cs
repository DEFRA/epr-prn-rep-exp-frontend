using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;

namespace Epr.Reprocessor.Exporter.UI.Validations.Attributes;

/// <summary>
/// Custom validation attribute that handles validation rules for tonnage.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class TonnageValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// The minimum allowed value for the weight (inclusive).
    /// </summary>
    /// <value>Default: 1</value>
    public int MinimumValue { get; set; } = 1;

    /// <summary>
    /// The maximum allowed value for the weight (inclusive).
    /// </summary>
    /// <value>Default: 10000000</value>
    public int MaximumValue { get; set; } = 10000000;

    /// <inheritdoc/>>.
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
        {
            // We don't want to handle this here as it is handled by the Required attribute.
            return ValidationResult.Success;
        }

        if (int.TryParse(value.ToString(), NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
        {
            if (result < MinimumValue)
            {
                return new ValidationResult(MaterialPermitInput.maximum_weight_minimum_error_message);
            }

            if (result > MaximumValue)
            {
                return new ValidationResult(MaterialPermitInput.maximum_weight_maximum_error_message);
            }
        }
        else
        {
            return new ValidationResult(MaterialPermitInput.maximum_weight_format_error_message);
        }

        return ValidationResult.Success;
    }
}