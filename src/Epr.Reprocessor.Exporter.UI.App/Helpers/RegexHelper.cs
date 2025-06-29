using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.App.Helpers;

public static class RegexHelper
{
    private static readonly Regex EndDigitsRegex = new(
        @"\d+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));

    private static readonly Regex UKPostcodeRegex = new(
        @ValidationRegExConstants.Postcode,
        RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));

    private static readonly Regex AlphaNumericRegex = new(
        @"^[a-zA-Z0-9]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));

    private static readonly Regex ContainsNumberRegex = new(
        @"\d+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));

    private static readonly Regex PermitNumberRegex = new(
        @"^EPR/[A-Z]{2}\d{4}[A-Z]{2}(?:/V\d{3})?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));

    public static int CountEndingDigits(string input)
    {
        if (string.IsNullOrEmpty(input))
            return 0;

        var match = EndDigitsRegex.Match(input);
        return match.Success ? match.Value.Length : 0;
    }

    public static bool ValidateUKPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode)) return false;

        postcode = postcode.ToUpper().Replace(" ", string.Empty);

        return UKPostcodeRegex.IsMatch(postcode);
    }

    public static bool IsAlphaNumeric(string input)
    {
        return AlphaNumericRegex.IsMatch(input);
    }

    public static bool ContainsNumber(string input)
    {
        return ContainsNumberRegex.IsMatch(input);
    }

    public static bool ValidatePermitNumber(string input)
    {
        return PermitNumberRegex.IsMatch(input);
    }
}
