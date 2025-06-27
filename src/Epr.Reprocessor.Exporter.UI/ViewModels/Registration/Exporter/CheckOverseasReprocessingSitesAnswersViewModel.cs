using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class CheckOverseasReprocessingSitesAnswersViewModel
{
    public CheckOverseasReprocessingSitesAnswersViewModel()
    {
        
    }

    public CheckOverseasReprocessingSitesAnswersViewModel(OverseasReprocessingSites sites)
    {
        OverseasAddresses = sites.OverseasAddresses;
    }

    public Guid RegistrationMaterialId { get; set; }

    public List<OverseasAddress>? OverseasAddresses { get; set; }
}
