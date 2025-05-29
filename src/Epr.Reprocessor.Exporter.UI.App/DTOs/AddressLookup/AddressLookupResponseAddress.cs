using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents the detailed components of a single address returned by the address lookup service.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressLookupResponseAddress
{
    /// <summary>
    /// The full address as a single formatted line.
    /// </summary>
    public string? AddressLine { get; init; }

    /// <summary>
    /// A sub-building name such as a flat or unit number.
    /// </summary>
    public string? SubBuildingName { get; init; }

    /// <summary>
    /// The name of the building, if applicable.
    /// </summary>
    public string? BuildingName { get; init; }

    /// <summary>
    /// The number of the building on the street.
    /// </summary>
    public string? BuildingNumber { get; init; }

    /// <summary>
    /// The name of the street.
    /// </summary>
    public string? Street { get; init; }

    /// <summary>
    /// The locality within the town or city.
    /// </summary>
    public string? Locality { get; init; }

    /// <summary>
    /// A further subdivision of the locality, if present.
    /// </summary>
    public string? DependentLocality { get; init; }

    /// <summary>
    /// The town or city.
    /// </summary>
    public string? Town { get; init; }

    /// <summary>
    /// The county or administrative region.
    /// </summary>
    public string? County { get; init; }

    /// <summary>
    /// The postal code.
    /// </summary>
    public string? Postcode { get; init; }

    /// <summary>
    /// The country name.
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// The X coordinate (e.g., easting) for GIS systems.
    /// </summary>
    public int? XCoordinate { get; init; }

    /// <summary>
    /// The Y coordinate (e.g., northing) for GIS systems.
    /// </summary>
    public int? YCoordinate { get; init; }

    /// <summary>
    /// The Unique Property Reference Number.
    /// </summary>
    public string? UPRN { get; init; }

    /// <summary>
    /// The match quality or status code.
    /// </summary>
    public string? Match { get; init; }

    /// <summary>
    /// A description of the match quality or reason.
    /// </summary>
    public string? MatchDescription { get; init; }

    /// <summary>
    /// The language in which the address is returned (e.g., "EN" or "CY").
    /// </summary>
    public string? Language { get; init; }
}
