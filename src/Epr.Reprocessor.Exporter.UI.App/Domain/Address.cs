namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents an address object.
/// </summary>
/// <param name="AddressLine1">The first line of the address.</param>
/// <param name="AddressLine2">The second line of the address.</param>
/// <param name="Locality">The locality of the address.</param>
/// <param name="Town">The town of the address.</param>
/// <param name="County">The county of the address.</param>
/// <param name="Country">The country of the address.</param>
/// <param name="Postcode">The postcode of the address.</param>
/// <param name="AddressId">The Address Id of the address.</param>
[ExcludeFromCodeCoverage]
public record Address(
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string Town,
    string? County,
    string? Country,
    string Postcode,
    int? AddressId = null);