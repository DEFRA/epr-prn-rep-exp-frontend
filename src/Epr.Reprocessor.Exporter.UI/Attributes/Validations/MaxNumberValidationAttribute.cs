using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxNumberValidationAttribute: ValidationAttribute
    {
        private const string REGEX_MAXNUMBERS = "^\\d{1,10}$";
        public int MaxCharacters { get; set; } = 10;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = value?.ToString() ?? string.Empty;

            if ((text.Length > MaxCharacters))
            {
                return Regex.IsMatch(text, REGEX_MAXNUMBERS) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
