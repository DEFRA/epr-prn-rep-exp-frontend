using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents a list of structured postal addresses.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressList"/> class.
    /// </summary>
    public AddressList()
    {
        Addresses = new List<Address>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressList"/> class from an address lookup response.
    /// </summary>
    /// <param name="addressLookupResponse">The address lookup response containing address data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="addressLookupResponse"/> is null.</exception>
    public AddressList(AddressLookupResponse addressLookupResponse)
    {
        ArgumentNullException.ThrowIfNull(addressLookupResponse);

        Addresses = addressLookupResponse.Results.Select(item => new Address
        {
            AddressSingleLine = item.Address.AddressLine,
            SubBuildingName = item.Address.SubBuildingName,
            BuildingName = item.Address.BuildingName,
            BuildingNumber = item.Address.BuildingNumber,
            Street = item.Address.Street,
            Town = item.Address.Town,
            County = item.Address.County,
            Postcode = item.Address.Postcode,
            Locality = item.Address.Locality,
            DependentLocality = item.Address.DependentLocality,
            IsManualAddress = false
        }).ToList();
    }

    /// <summary>
    /// Gets or sets the collection of addresses.
    /// </summary>
    public IList<Address> Addresses { get; set; }
}
