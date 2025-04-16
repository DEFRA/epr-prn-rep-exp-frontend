using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxNumberValidationAttribute: ValidationAttribute
    {
        public string Regex { get; set; } = "^w*[\\d{0,12}$]";
        public int MaxCharacters { get; set; } = 12;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = value?.ToString() ?? string.Empty;

            if ((text.Length > MaxCharacters))
            {
				return System.Text.RegularExpressions.Regex.IsMatch(text, Regex, RegexOptions.None, TimeSpan.FromSeconds(250))
					? ValidationResult.Success
					: new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
        }
    }
}
