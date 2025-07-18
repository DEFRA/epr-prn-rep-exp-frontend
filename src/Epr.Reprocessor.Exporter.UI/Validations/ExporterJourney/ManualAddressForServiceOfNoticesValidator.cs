using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using FluentValidation;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;

namespace Epr.Reprocessor.Exporter.UI.Validations.ExporterJourney
{
    public class ManualAddressForServiceOfNoticesValidator : AbstractValidator<ManualAddressForServiceOfNoticesViewModel>
    {

        public ManualAddressForServiceOfNoticesValidator()
        {
            // Address
            Include(new AddressValidator());
        }
    }

}
