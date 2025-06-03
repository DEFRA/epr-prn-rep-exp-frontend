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
    /// The service of notice details for the reprocessing site, including where notices should be sent and the type of address used.
    /// </summary>
    public ServiceOfNotice? ServiceOfNotice { get; set; } = new();

    /// <summary>
    /// The grid reference of the reprocessing site.
    /// </summary>
    public string SiteGridReference { get; set; } = null!;

    /// <summary>
    /// The nation the address falls within.
    /// ///</summary>
    public UkNation Nation { get; set; }

    /// <summary>
    /// The source page that this reprocessing site was created from, used for navigation purposes.
    /// </summary>
    public string SourcePage { get; set; } = null!;

    /// <summary>
    /// The list of addresses found at a postcode and selected address
    /// </summary>
    public LookupAddress? LookupAddress { get; set; } = new();

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
    
    /// <summary>
    /// Sets the site grid reference for the reprocessing site.
    /// </summary>
    /// <param name="siteGridReference">The grid reference of the reprocessing site.</param>
    /// <returns>This instance.</returns>
    public ReprocessingSite SetSiteGridReference(string siteGridReference)
    {
        SiteGridReference = siteGridReference;

        return this;
    }

    /// <summary>
    /// Sets the nation of the reprocessing site.
    /// </summary>
    /// <param name="nation">The value to set.</param>
    /// <returns>This instance.</returns>
    public ReprocessingSite SetNation(UkNation nation)
    {
        Nation = nation;

        return this;
    }

    /// <summary>
    /// Sets the page that we came from when creating this reprocessing site, used for navigation purposes.
    /// </summary>
    /// <param name="sourcePage">The page we came from.</param>
    /// <returns>This instance.</returns>
    public ReprocessingSite SetSourcePage(string sourcePage)
    {
        SourcePage = sourcePage;

        return this;
    }
}