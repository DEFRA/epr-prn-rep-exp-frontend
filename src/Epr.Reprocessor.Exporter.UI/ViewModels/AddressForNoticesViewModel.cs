using System.ComponentModel.DataAnnotations; 
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Domain;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class AddressForNoticesViewModel
{    
    [Required(ErrorMessage = "Select an address for service of notices.")]
    public AddressOptions? SelectedOption { get; set; }
        
    public AddressViewModel? BusinessAddress { get; set; }

    public AddressViewModel? RegisteredAddress { get; set; }

    public AddressViewModel? SiteAddress { get; set; }

    public AddressForNoticesViewModel SetAddress(Address? address, AddressOptions? addressOptions)
    {
        if(addressOptions is AddressOptions.RegisteredAddress)
        {
            RegisteredAddress = MapAddress(address);
            BusinessAddress = null;
            SiteAddress = null;
            SelectedOption = AddressOptions.RegisteredAddress;
        }
        else if (addressOptions is AddressOptions.BusinessAdress)
        {
            BusinessAddress = MapAddress(address);
            RegisteredAddress = null;
            SiteAddress = null;
            SelectedOption = AddressOptions.BusinessAdress;
        }
        else if (addressOptions is AddressOptions.SiteAddress)
        {
            SiteAddress = MapAddress(address);
            RegisteredAddress = null;
            BusinessAddress = null;
            SelectedOption = AddressOptions.SiteAddress;
        }
        else
        {
            SelectedOption = AddressOptions.DifferentAddress;
        }

        return this;
    }

    public Address? GetAddress() =>
       SelectedOption switch
       {
           AddressOptions.RegisteredAddress => MapAddress(RegisteredAddress),
           AddressOptions.BusinessAdress => MapAddress(BusinessAddress),
           AddressOptions.SiteAddress => MapAddress(BusinessAddress),
           _ => MapAddress(RegisteredAddress ?? BusinessAddress)
       };

    private Address? MapAddress(AddressViewModel? addressToMap)
    {
        if (addressToMap is null)
        {
            return null;
        }

        return new(addressToMap.AddressLine1, addressToMap.AddressLine2, null, addressToMap.TownOrCity,
            addressToMap.County, null, addressToMap.Postcode, UkNation.England);
    }

    private AddressViewModel? MapAddress(Address? addressToMap)
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
}

