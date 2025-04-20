using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxNumberValidationAttribute: ValidationAttribute
    {
        public string Regex { get; set; } = ValidationRegExConstants.GridReference10DigitMax;
        public int MaxCharacters { get; set; } = 10;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var text = value?.ToString() ?? string.Empty;
            var digitValues = ExtractIntValuesFromString(text);

            if ((digitValues.Length > MaxCharacters))
            {
				return System.Text.RegularExpressions.Regex.IsMatch(digitValues, Regex, RegexOptions.None, TimeSpan.FromSeconds(250))
					? ValidationResult.Success
					: new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
        }

        private static string ExtractIntValuesFromString(string value) {
            var ram = GetNumbersFromStringRegEx.GetValue(value);
            return ram;
        }
    }

    [ExcludeFromCodeCoverage]
    public static partial class GetNumbersFromStringRegEx
    {
        [GeneratedRegex(@"\d+", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 250)]
        private static partial Regex MatchNumber();

        public static string GetValue(string value) => MatchNumber().Match(value).Value;
    }

}
