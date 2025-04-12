using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    public class MaxNumberValidationAttribute: ValidationAttribute
    {
        private const string REGEX_MAXNUMBERS = "^!*(\\d!*){0,10}$";
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var charValue = value?.ToString() ?? string.Empty;

            return Regex.IsMatch(charValue, REGEX_MAXNUMBERS) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
