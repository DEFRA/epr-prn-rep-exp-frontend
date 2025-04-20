using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchAtLeastValidationAttribute : ValidationAttribute
    {
        private string RegEx { get; set; } = ValidationRegExConstants.GridReferenceNumbers;
        public int MaxCharactersOfNumbersToMatch { get; set; } = 4;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           var regExWithMaxNumbers = $"{RegEx}{{{MaxCharactersOfNumbersToMatch},}}";
           var text = value?.ToString() ?? string.Empty;

           if (value is null || value == string.Empty) return ValidationResult.Success;

           var digitValues = ExtractIntValuesFromString(text);

           if(digitValues.Length == 0) return ValidationResult.Success;

            return Regex.IsMatch(digitValues, regExWithMaxNumbers, RegexOptions.None, TimeSpan.FromSeconds(250))
                    ? ValidationResult.Success
                    : new ValidationResult(ErrorMessage);
        }

        private static string ExtractIntValuesFromString(string value)
        {
            var ram = GetNumbersFromStringRegEx.GetValue(value);
            return ram;
        }
    }
}
