using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Domain;
using Epr.Reprocessor.Exporter.UI.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

public class AddressViewModel
{
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
