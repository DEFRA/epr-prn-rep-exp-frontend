using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Enums;

/// <summary>
/// Defines values for the type of address being provided.
/// </summary>
public enum AddressOptions
{
    /// <summary>
    /// No value set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The address is registered on an external source such as Companies house.
    /// </summary>
    [Description("The registered address")]
    [Display(Name = "The registered address")]
    RegisteredAddress = 1,

    /// <summary>
    /// The address is a business address and not one found on companies house.
    /// </summary>
    [Description("The site address")]
    [Display(Name = "The site address")]
    SiteAddress = 2,

    /// <summary>
    /// A different address is being provided, one we don't have on file or one that is different to what we have presented to the user.
    /// </summary>
    [Description("It's a different address")]
    [Display(Name = "It's a different address")]
    DifferentAddress = 3 
}
