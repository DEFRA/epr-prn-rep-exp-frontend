using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchAtLeastValidationAttribute : ValidationAttribute
    {
        public MatchAtLeastValidationAttribute() { 
           RegEx = $"{RegEx}{{{MaxCharactersOfNumbersToMatch},}}";
        }

        private string RegEx { get; set; } = ValidationRegExConstants.GridReferenceNumbers;
        public int MaxCharactersOfNumbersToMatch { get; set; } = 4;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return ValidationResult.Success;

           var numericValues = ExtractIntValuesFromString(text);
            if (string.IsNullOrEmpty(numericValues))
                return ValidationResult.Success;

            return Regex.IsMatch(numericValues, RegEx, RegexOptions.None, TimeSpan.FromSeconds(250))
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
