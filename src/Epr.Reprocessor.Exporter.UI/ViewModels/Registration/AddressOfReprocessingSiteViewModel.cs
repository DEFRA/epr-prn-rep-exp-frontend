using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class AddressOfReprocessingSiteViewModel
{
    public AddressOptions? SelectedOption { get; set; }

    public AddressViewModel? BusinessAddress { get; set; }

    public AddressViewModel? RegisteredAddress { get; set; }

    public Address? ChosenAddress =>
        SelectedOption switch
        {
            AddressOptions.RegisteredAddress => MapAddress(RegisteredAddress),
            AddressOptions.SiteAddress => MapAddress(BusinessAddress),
            _ => null
        };

    private static Address? MapAddress(AddressViewModel? addressToMap)
    {
        if (addressToMap is null)
        {
            return null;
        }

        return new(addressToMap.AddressLine1, addressToMap.AddressLine2, null, addressToMap.TownOrCity,
            addressToMap.County, null, addressToMap.Postcode, UkNation.England);
    }

    private static AddressViewModel? MapAddress(Address? addressToMap)
    {
        if (addressToMap is null)
        {
            return null;
        }

        return new AddressViewModel
        {
            AddressLine1 = addressToMap.AddressLine1,
            AddressLine2 = addressToMap.AddressLine2,
            TownOrCity = addressToMap.Town,
            County = addressToMap.County,
            Postcode = addressToMap.Postcode,
        };
    }

    public AddressOfReprocessingSiteViewModel SetAddress(Address address, AddressOptions? addressOptions)
    {
        if (addressOptions is AddressOptions.RegisteredAddress)
        {
            RegisteredAddress = MapAddress(address);
            BusinessAddress = null;
            SelectedOption = AddressOptions.RegisteredAddress;
        }
        else
        {
            BusinessAddress = MapAddress(address);
            RegisteredAddress = null;
            SelectedOption = AddressOptions.SiteAddress;
        }

        return this;
    }
}