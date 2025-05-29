using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;
using Epr.Reprocessor.Exporter.UI.App.Enums;
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
   // public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    /// <summary>
    /// This is the business address
    /// </summary>
    public Address? BusinessAddress { get; set; }

    /// <summary>
    /// This is the registered address
    /// </summary>

    public Address? RegisteredAddress { get; set; }
    
    /// <summary>
    /// This is the site address
    /// </summary>
    public Address? SiteAddress { get; set; }

    /// <summary>
    /// This is a different address that is not the business, registered or site address.
    /// </summary>
    public Address? DifferentAddress { get; set; }

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


    public Address GetAddress(AddressOptions? typeOfAddress)
    {
        if (typeOfAddress is AddressOptions.BusinessAdress)
        {
            return BusinessAddress;
        }
        else if (typeOfAddress is AddressOptions.RegisteredAddress)
        {
            return RegisteredAddress;
        }
        else if (typeOfAddress is AddressOptions.SiteAddress)
        {
            return SiteAddress;
        }
        else
        {
            return DifferentAddress;
        }
    }

    /// <summary>
    /// Sets the address for the reprocessing site.
    /// </summary>
    /// <param name="address">The address of the reprocessing site.</param>
    /// <param name="typeOfAddress">The type of address being set, i.e a registered or business address.</param>
    /// <returns>This instance.</returns>
    public ReprocessingSite SetReprocessingSite(Address? address, AddressOptions? typeOfAddress)
    {
        TypeOfAddress = typeOfAddress;

        if (typeOfAddress is AddressOptions.BusinessAdress)
        {
            BusinessAddress = address;
        }
        else if (typeOfAddress is AddressOptions.RegisteredAddress)
        {
            RegisteredAddress = address;
        }
        else if (typeOfAddress is AddressOptions.SiteAddress)
        {
            SiteAddress = address;
        }
        else
        {
            DifferentAddress = address;
        }

        //Address = address;
        //TypeOfAddress = typeOfAddress;

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