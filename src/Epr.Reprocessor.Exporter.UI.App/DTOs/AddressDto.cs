namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public record AddressDto
{
    public int? Id { get; set; } = 0;

    public string AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; } = string.Empty;

    public string TownCity { get; set; }

    public string? County { get; set; } = string.Empty;

    public string? Country { get; set; } = string.Empty;

    public string PostCode { get; set; }

    public int? NationId { get; set; } = 0;

    public string GridReference { get; set; }

    public virtual bool Equals(AddressDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return IsEqual(other.AddressLine1, AddressLine1) &&
               IsEqual(other.AddressLine2, AddressLine2) &&
               IsEqual(other.TownCity, TownCity) &&
               IsEqual(other.Country, Country) &&
               IsEqual(other.County, County);
    }

    private static bool IsEqual(string? a, string? b)
    {
        a = ReplaceEmptyWithNull(a);
        b = ReplaceEmptyWithNull(b);

        if (a is null && b is null)
        {
            return true;
        }
        if (a is null || b is null)
        {
            return false;
        }
        return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
    }

    private static string? ReplaceEmptyWithNull(string? input)
    {
        if (input is "")
        {
            return null;
        }

        return input;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(AddressLine1);
        hashCode.Add(AddressLine2);
        hashCode.Add(TownCity);
        hashCode.Add(County);
        hashCode.Add(Country);
        hashCode.Add(PostCode);
        return hashCode.ToHashCode();
    }
}