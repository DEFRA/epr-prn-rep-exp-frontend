using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Filters;

public class ValidUKPostcodeAttribute : ValidationAttribute
{
    private static readonly Regex _ukPostcodeRegex = new Regex(
        @"^(GIR 0AA|[A-PR-UWYZ][A-HK-Y0-9][A-HJKS-UW0-9]?[0-9][ABD-HJLNP-UW-Z]{2})$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; 

        string postcode = value.ToString().ToUpper().Replace(" ", "");

        if (_ukPostcodeRegex.IsMatch(postcode))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Invalid UK postcode format.");
    }
}
