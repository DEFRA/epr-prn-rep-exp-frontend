namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class DecimalExtensionMethods
{
    public static string ToStringWithOutDecimalPlaces(this decimal value, int decimalPlaces = 0)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be negative.");
        }

        return value.ToString($"F{decimalPlaces}");
    }
}
