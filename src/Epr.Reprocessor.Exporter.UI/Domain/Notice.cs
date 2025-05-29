using Epr.Reprocessor.Exporter.UI.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents the address details for notices related to the reprocessing site/ notice.
/// </summary>
[ExcludeFromCodeCoverage]
public class Notice
{
    /// <summary>
    /// The address of the reprocessing site.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    public Notice SetNoticeAddress(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }
}
