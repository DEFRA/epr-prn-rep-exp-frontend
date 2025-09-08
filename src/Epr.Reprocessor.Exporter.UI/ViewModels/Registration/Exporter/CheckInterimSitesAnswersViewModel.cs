using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

public class CheckInterimSitesAnswersViewModel
{
    public CheckInterimSitesAnswersViewModel(OverseasMaterialReprocessingSite sites)
    {
        OverseasAddress = sites.OverseasAddress;
        InterimSiteAddresses = sites.InterimSiteAddresses;
    }

    public OverseasAddressBase OverseasAddress { get; init; }
    public List<InterimSiteAddress> InterimSiteAddresses { get; set; }
}
