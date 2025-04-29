using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Enums;

public enum AddressOptions
{
    [Description("The registered address")]
    [Display(Name = "The registered address")]
    RegisteredAddress = 1,
    [Description("The site address")]
    [Display(Name = "The site address")]
    SiteAddress = 2,
    [Description("It's a different address")]
    [Display(Name = "It's a different address")]
    DifferentAddress = 3 

}
