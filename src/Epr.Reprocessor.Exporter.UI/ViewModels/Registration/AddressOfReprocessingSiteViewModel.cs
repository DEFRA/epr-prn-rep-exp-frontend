using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Domain;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// Defines a model used to power the view for the address of the reprocessing site page.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressOfReprocessingSiteViewModel
{
    #region Properties

    /// <summary>
    /// The option selected as to which address is being selected as the reprocessing site.
    /// </summary>
    public AddressOptions? SelectedOption { get; set; }

    /// <summary>
    /// The business address of the reprocessing site, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.SiteAddress"/>.
    /// </summary>
    public AddressViewModel? BusinessAddress { get; set; }

    /// <summary>
    /// The registered address of the reprocessing site, only set if the <see cref="SelectedOption"/> is <see cref="AddressOptions.RegisteredAddress"/>.
    /// </summary>
    public AddressViewModel? RegisteredAddress { get; set; }

    #endregion

    #region Main Api

    /// <summary>
    /// Sets the correct address fields based on the type of address being provided.
    /// </summary>
    /// <param name="address">The address details to set.</param>
    /// <param name="addressOptions">The type of address selected and being set.</param>
    /// <returns>The updated view model.</returns>
    public AddressOfReprocessingSiteViewModel SetAddress(Address? address, AddressOptions? addressOptions)
    {
        if (addressOptions is AddressOptions.RegisteredAddress)
        {
            RegisteredAddress = MapAddress(address);
            BusinessAddress = null;
            SelectedOption = AddressOptions.RegisteredAddress;
        }
        else if (addressOptions is AddressOptions.BusinessAdress)
        {
            BusinessAddress = MapAddress(address);
            RegisteredAddress = null;
            SelectedOption = AddressOptions.BusinessAdress;
        }
        else if (addressOptions is AddressOptions.SiteAddress)
        {
            BusinessAddress = MapAddress(address);
            RegisteredAddress = null;
            SelectedOption = AddressOptions.SiteAddress;
        }
        else
        {
            SelectedOption = AddressOptions.DifferentAddress;
        }
        
        return this;
    }

    /// <summary>
    /// Gets the correct address fields based on the type of address being provided.
    /// </summary>
    /// <returns>The set address.</returns>
    public Address? GetAddress() =>
        SelectedOption switch
        {
            AddressOptions.RegisteredAddress => MapAddress(RegisteredAddress),
            AddressOptions.BusinessAdress => MapAddress(BusinessAddress),           
            _ => MapAddress(RegisteredAddress ?? BusinessAddress)
        };

    #endregion

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