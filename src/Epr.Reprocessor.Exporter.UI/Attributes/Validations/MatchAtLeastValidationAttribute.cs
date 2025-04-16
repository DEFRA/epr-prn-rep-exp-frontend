using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchAtLeastValidationAttribute : ValidationAttribute
    {
        public string RegEx { get; set; } = "{4}";
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = value?.ToString() ?? string.Empty;

           return Regex.IsMatch(text, RegEx, RegexOptions.None, TimeSpan.FromSeconds(250))
                    ? ValidationResult.Success
                    : new ValidationResult(ErrorMessage);
        }
    }
}
