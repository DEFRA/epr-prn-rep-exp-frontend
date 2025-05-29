using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents metadata about the address lookup request and response.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressLookupResponseHeader
{
    /// <summary>
    /// The original search query submitted to the lookup service.
    /// </summary>
    public string? Query { get; init; }

    /// <summary>
    /// The offset value used for paginating the results.
    /// </summary>
    public string? Offset { get; init; }

    /// <summary>
    /// The total number of results found by the lookup service.
    /// </summary>
    public string? TotalResults { get; init; }

    /// <summary>
    /// The format used for the response (e.g., JSON or XML).
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// The name of the dataset used for the lookup.
    /// </summary>
    public string? Dataset { get; init; }

    /// <summary>
    /// The language requested (e.g., "EN" for English or "CY" for Welsh).
    /// </summary>
    public string? Lr { get; init; }

    /// <summary>
    /// The maximum number of results requested by the client.
    /// </summary>
    public string? MaxResults { get; init; }

    /// <summary>
    /// The number of results that matched the search query.
    /// </summary>
    public string? MatchingTotalResults { get; init; }
}
