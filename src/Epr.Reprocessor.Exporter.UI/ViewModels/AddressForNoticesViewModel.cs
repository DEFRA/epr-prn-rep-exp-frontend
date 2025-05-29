using System.ComponentModel.DataAnnotations; 
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Domain;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class AddressForNoticesViewModel
{    
    public AddressOptions? SelectedAddressOptions { get; set; }
    
    public AddressViewModel? BusinessAddress { get; set; }

    public AddressViewModel? SiteAddress { get; set; }

    public bool ShowSiteAddress { get; set; }
   
    public Address? GetAddress() =>
        SelectedAddressOptions switch
        {
            AddressOptions.RegisteredAddress => MapAddress(BusinessAddress),
            AddressOptions.BusinessAddress => MapAddress(BusinessAddress),
            AddressOptions.SiteAddress => MapAddress(SiteAddress),
            AddressOptions.DifferentAddress => MapAddress(SiteAddress)           
        };

    #region Mapping methods

    private static Address? MapAddress(AddressViewModel? addressToMap)
    {
        if (addressToMap is null)
        {
            return null;
        }

        return new(addressToMap.AddressLine1, addressToMap.AddressLine2, null, addressToMap.TownOrCity,
            addressToMap.County, null, addressToMap.Postcode);
    }   
    #endregion
}

