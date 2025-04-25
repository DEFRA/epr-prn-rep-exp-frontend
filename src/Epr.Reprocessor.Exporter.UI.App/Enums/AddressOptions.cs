using System.ComponentModel;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

public enum AddressOptions
{
    [Description("The registered address")]
    RegisteredAddress = 1,
    [Description("The site address")]
    SiteAddress = 2,
    [Description("It's a different address")]
    DifferentAddress = 3 

}
