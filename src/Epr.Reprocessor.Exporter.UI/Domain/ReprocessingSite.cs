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

    public string GridReference { get; set; }

    /// <summary>
    /// Sets the address for the reprocessing site.
    /// </summary>
    /// <param name="address">The address of the reprocessing site.</param>
    /// <param name="typeOfAddress">The type of address being set, i.e a registered or business address.</param>
    /// <returns>This instance.</returns>
    public ReprocessingSite SetAddress(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }  
    
    public ReprocessingSite SetGridReference(string gridReference)
    {
        GridReference = gridReference;
        return this;
    }
}