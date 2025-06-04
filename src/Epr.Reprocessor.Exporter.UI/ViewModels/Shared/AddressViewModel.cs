namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

[ExcludeFromCodeCoverage]
public class AddressViewModel
{
    public AddressViewModel()
    {

    }

    public AddressViewModel(Address? address)
    {
        AddressLine1 = address?.AddressLine1 ?? string.Empty;
        AddressLine2 = address?.AddressLine2 ?? string.Empty;
        TownOrCity = address?.Town ?? string.Empty;
        County = address?.County ?? string.Empty;
        Postcode = address?.Postcode ?? string.Empty;
    }

    public string AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string TownOrCity { get; set; }

    public string? County { get; set; }

    public string Postcode { get; set; }

    public string FormattedAddress => string.Join(", ", new[] { AddressLine1, AddressLine2, TownOrCity, County, Postcode }
                                      .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));

    public Address? GetAddress() => MapAddress(this);

    private static Address MapAddress(AddressViewModel addressToMap)
    {
        return new(addressToMap.AddressLine1, addressToMap.AddressLine2, null, addressToMap.TownOrCity,
            addressToMap.County, null, addressToMap.Postcode);
    }
}
