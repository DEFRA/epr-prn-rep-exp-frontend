using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents a manually selected or entered address associated with a postcode.
/// </summary>
[ExcludeFromCodeCoverage]
public class LookupAddress
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupAddress"/> class.
    /// This constructor is primarily for serialization or model binding purposes.
    /// </summary>
    public LookupAddress()
    {
        AddressesForPostcode = new List<Address>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupAddress"/> class using provided address data.
    /// </summary>
    /// <param name="postcode">The postcode for address lookup.</param>
    /// <param name="addressList">The list of addresses associated with the postcode.</param>
    /// <param name="selectedAddressIndex">Optional selected address index.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="addressList"/> is null.</exception>
    public LookupAddress(string? postcode, AddressList addressList, int? selectedAddressIndex = null)
    {
        ArgumentNullException.ThrowIfNull(addressList);

        Postcode = postcode ?? string.Empty;
        AddressesForPostcode = addressList.Addresses
            .Select(address => new Address(
                address.BuildingNumberAndStreet,
                address.Street,
                address.Locality,
                address.Town,
                address.County,
                address.Country,
                address.Postcode))
            .ToList();

        SelectedAddressIndex =
            selectedAddressIndex.HasValue && selectedAddressIndex.Value < AddressesForPostcode.Count
            ? selectedAddressIndex
            : null;

    }

    /// <summary>
    /// Gets or sets the postcode used for address lookup.
    /// </summary>
    public string? Postcode { get; set; }

    /// <summary>
    /// Gets or sets the list of addresses associated with the postcode.
    /// </summary>
    public List<Address> AddressesForPostcode { get; set; }

    /// <summary>
    /// Gets or sets the index of the selected address from the list.
    /// </summary>
    public int? SelectedAddressIndex { get; set; }

    /// <summary>
    /// Gets the currently selected address based on <see cref="SelectedAddressIndex"/>.
    /// Returns null if the index is invalid or out of bounds.
    /// </summary>
    public Address? SelectedAddress =>
        SelectedAddressIndex.HasValue &&
        SelectedAddressIndex.Value >= 0 &&
        SelectedAddressIndex.Value < AddressesForPostcode.Count
            ? AddressesForPostcode[SelectedAddressIndex.Value]
            : null;
}