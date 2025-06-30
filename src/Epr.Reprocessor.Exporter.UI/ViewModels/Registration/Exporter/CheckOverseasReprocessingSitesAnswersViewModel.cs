using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class CheckOverseasReprocessingSitesAnswersViewModel
{
    public CheckOverseasReprocessingSitesAnswersViewModel()
    {

    }

    public CheckOverseasReprocessingSitesAnswersViewModel(ExporterRegistrationApplicationSession session)
    {
        RegistrationMaterialId = session.RegistrationMaterialId ?? Guid.NewGuid();
        OverseasAddresses = session.OverseasReprocessingSites.OverseasAddresses;
    }

    public Guid RegistrationMaterialId { get; set; }

    public List<OverseasAddress>? OverseasAddresses { get; set; }
}