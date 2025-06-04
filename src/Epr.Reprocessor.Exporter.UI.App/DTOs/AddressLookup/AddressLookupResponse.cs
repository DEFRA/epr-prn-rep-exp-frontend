using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents the response from an address lookup service,
/// containing metadata and the list of address results.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressLookupResponse
{
    /// <summary>
    /// Gets or sets the response header, which includes metadata about the request.
    /// </summary>
    public required AddressLookupResponseHeader Header { get; init; }

    /// <summary>
    /// Gets or sets the list of address lookup results.
    /// </summary>
    public required AddressLookupResponseResult[] Results { get; init; }
}
