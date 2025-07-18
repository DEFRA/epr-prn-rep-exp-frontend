using System.Globalization;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class StringExtensions
{
    public static bool TryConvertToLong(this string? value, out long convertedValue)
    {
        return long.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedValue);
    }

    public static bool TryConvertToInt(this string? value, out int convertedValue)
    {
        return int.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedValue);
    }

    public static int ConvertToInt(this string? value)
    {
        value.TryConvertToInt(out var convertedValue);
        
        return convertedValue;
    }
}
