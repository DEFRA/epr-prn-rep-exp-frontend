using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Domain;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class AddressForNoticesViewModel
{
    #region Properties
    /// <summary>
    /// The option selected as to which address is being selected as the reprocessing site.
    /// </summary>
    public AddressOptions? SelectedOption { get; set; }

    /// <summary>
    /// The business address of the reprocessing site, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.BusinessAdress"/> or <see cref="AddressOptions.SiteAddress"/>.
    /// </summary>

    public AddressViewModel? BusinessAddress { get; set; }

    /// <summary>
    /// The registered address of the reprocessing site, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.RegisteredAddress"/> or <see cref="AddressOptions.SiteAddress"/>.
    /// </summary>
    public AddressViewModel? RegisteredAddress { get; set; }

    /// <summary>
    /// The site address of the reprocessing site, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.SiteAddress"/>.
    /// </summary>
    public AddressViewModel? SiteAddress { get; set; }

    /// <summary>
    /// A different address that is not the business, registered or site address, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.DifferentAddress"/>.
    /// </summary>
    public AddressViewModel? DifferentAddress { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ShowSiteAddress { get; set; }

    #endregion

    #region Main Api
    /// <summary>
    /// Sets the correct address fields based on the type of address being provided.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="addressOptions"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the address based on the selected option.
    /// </summary>
    /// <returns></returns>
    public Address? GetAddress() =>
       SelectedOption switch
       {
           AddressOptions.RegisteredAddress => MapAddress(RegisteredAddress),
           AddressOptions.BusinessAdress => MapAddress(BusinessAddress),
           AddressOptions.SiteAddress => MapAddress(SiteAddress),
           AddressOptions.DifferentAddress => MapAddress(DifferentAddress),
           _ => MapAddress(RegisteredAddress ?? BusinessAddress)
       };
    #endregion

    #region Mapping methods
    private Address? MapAddress(AddressViewModel? addressToMap)
    {
        if (addressToMap is null)
        {
            return null;
        }

        return new(addressToMap.AddressLine1, addressToMap.AddressLine2, null, addressToMap.TownOrCity,
            addressToMap.County, null, addressToMap.Postcode);
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

    #endregion
}

