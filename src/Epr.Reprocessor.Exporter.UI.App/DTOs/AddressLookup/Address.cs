using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents a postal address with structured components.
/// </summary>
[ExcludeFromCodeCoverage]
public class Address
{
    /// <summary>
    /// The complete address in a single formatted line.
    /// </summary>
    public string? AddressSingleLine { get; init; }

    /// <summary>
    /// The sub-building name, such as apartment or unit.
    /// </summary>
    public string? SubBuildingName { get; init; }

    /// <summary>
    /// The building name, if applicable.
    /// </summary>
    public string? BuildingName { get; init; }

    /// <summary>
    /// The building number on the street.
    /// </summary>
    public string? BuildingNumber { get; init; }

    /// <summary>
    /// The name of the street.
    /// </summary>
    public string? Street { get; init; }

    /// <summary>
    /// The town or city name.
    /// </summary>
    public string? Town { get; init; }

    /// <summary>
    /// The county or region.
    /// </summary>
    public string? County { get; init; }

    /// <summary>
    /// The country name.
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// The postal code.
    /// </summary>
    public string? Postcode { get; init; }

    /// <summary>
    /// The locality within the town or city.
    /// </summary>
    public string? Locality { get; init; }

    /// <summary>
    /// The dependent locality, typically a subdivision of the main locality.
    /// </summary>
    public string? DependentLocality { get; init; }

    /// <summary>
    /// Indicates whether this address was entered manually by a user.
    /// </summary>
    public bool IsManualAddress { get; init; }

    /// <summary>
    /// Gets the combined building number and street name in a formatted string.
    /// </summary>
    public string BuildingNumberAndStreet =>
        string.IsNullOrWhiteSpace(BuildingNumber) ? Street ?? string.Empty : $"{BuildingNumber} {Street}".Trim();

    /// <summary>
    /// Gets a list of key address fields in order for display or validation purposes.
    /// </summary>
    public string?[] AddressFields =>
    [
        SubBuildingName,
        BuildingName,
        BuildingNumberAndStreet,
        Town,
        County,
        Postcode
    ];
}
