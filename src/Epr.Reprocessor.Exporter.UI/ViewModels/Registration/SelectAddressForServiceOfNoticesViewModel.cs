using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class SelectAddressForServiceOfNoticesViewModel : LookupAddressViewModel
{
    public SelectAddressForServiceOfNoticesViewModel() : base()
    {

    }

    public SelectAddressForServiceOfNoticesViewModel(Domain.LookupAddress manualAddress) : base(manualAddress)
    {
    }
}
