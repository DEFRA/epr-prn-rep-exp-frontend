using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;

namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents the details of a reprocessing site.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReprocessingSite
{
    /// <summary>
    /// The address of the reprocessing site.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    public ReprocessingSite SetReprocessingSite(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }
}