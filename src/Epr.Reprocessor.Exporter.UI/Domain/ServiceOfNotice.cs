namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents the details of the notices and where notices should be sent for a reprocessing site.
/// </summary>
[ExcludeFromCodeCoverage]
public class ServiceOfNotice : IHasSourcePage<ServiceOfNotice>
{
    /// <summary>
    /// The address where notices should be sent.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    /// <summary>
    /// The list of addresses found at a postcode and selected address
    /// </summary>
    public LookupAddress? LookupAddress { get; set; } = new();

    /// <summary>
    /// The source page that the user came from, used for navigation purposes.
    /// </summary>
    public string? SourcePage { get; set; }

    /// <summary>
    /// Set the address.
    /// </summary>
    /// <param name="address">The address where notices should be sent.</param>
    /// <param name="typeOfAddress">The type of address being set, i.e a registered or business address.</param>
    /// <returns>This instance.</returns>
    public ServiceOfNotice SetAddress(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }

    /// <summary>
    /// Set the source page for the current instance, used for navigation purposes.
    /// </summary>
    /// <param name="page">The source page to set.</param>
    /// <returns>This instance.</returns>
    public ServiceOfNotice SetSourcePage(string page)
    {
        SourcePage = page;

        return this;
    }
}