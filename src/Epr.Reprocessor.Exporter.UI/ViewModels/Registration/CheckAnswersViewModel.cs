using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class CheckAnswersViewModel 
{
    public CheckAnswersViewModel()
    {
    }

    public CheckAnswersViewModel(ReprocessingSite reprocessingSite)
    {
        // Site Location
        SiteLocation = reprocessingSite.Nation;

        // Site Grid Reference
        SiteGridReference = reprocessingSite.SiteGridReference;

        // Reprocessing Site Address
        ReprocessingSiteAddress = new AddressViewModel(reprocessingSite.Address);

        // Service Of Notices Address
        ServiceOfNoticesAddress = new AddressViewModel(reprocessingSite.ServiceOfNotice.Address);
    }

    public UkNation? SiteLocation { get; set; }
    public AddressViewModel ReprocessingSiteAddress { get; set; }
    public string SiteGridReference { get; set; }
    public AddressViewModel ServiceOfNoticesAddress { get; set; }
}
