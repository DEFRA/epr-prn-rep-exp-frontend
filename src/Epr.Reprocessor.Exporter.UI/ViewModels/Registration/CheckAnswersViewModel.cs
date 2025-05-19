using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class CheckAnswersViewModel 
{
    public UkNation SiteLocation { get; set; }
    public AddressViewModel ReprocessingSiteAddress { get; set; }
    public string SiteGridReference { get; set; }
    public AddressViewModel ServiceOfNoticesAddress { get; set; }
}
