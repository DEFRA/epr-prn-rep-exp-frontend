using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class SelectAddressForReprocessingSiteViewModel : SelectAddressViewModel
{
    public SelectAddressForReprocessingSiteViewModel() : base()
    {

    }

    public SelectAddressForReprocessingSiteViewModel(Domain.LookupAddress manualAddress) : base(manualAddress)
    {
    }
}
