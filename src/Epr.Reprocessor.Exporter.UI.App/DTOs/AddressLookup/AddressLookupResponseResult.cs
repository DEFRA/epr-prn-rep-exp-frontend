using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

/// <summary>
/// Represents a single result from the address lookup service,
/// containing a structured address.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressLookupResponseResult
{
    /// <summary>
    /// Gets or sets the structured address returned in the result.
    /// </summary>
    public required AddressLookupResponseAddress Address { get; init; }
}
