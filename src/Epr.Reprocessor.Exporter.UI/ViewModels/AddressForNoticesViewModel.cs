namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class AddressForNoticesViewModel
{
    #region Properties
    /// <summary>
    /// The selected address options for the reprocessing site.
    /// </summary>
    public AddressOptions? SelectedAddressOptions { get; set; }

    /// <summary>
    /// The address that is registered on an external source such as Companies house.
    /// </summary>
    public AddressViewModel? BusinessAddress { get; set; }

    /// <summary>
    /// The address is site address, entered by the user or selected from a lookup.
    /// </summary>
    public AddressViewModel? SiteAddress { get; set; }

    /// <summary>
    /// Indicates whether the site address radio should be shown.
    /// </summary>
    public bool ShowSiteAddress { get; set; }

    /// <summary>
    /// Indicates whether the business address radio should be shown.
    /// </summary>
    public bool IsBusinessAddress { get; set; }
    #endregion


    /// <summary>
    /// Gets the address based on the selected address options.
    /// </summary>
    /// <returns></returns>
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

