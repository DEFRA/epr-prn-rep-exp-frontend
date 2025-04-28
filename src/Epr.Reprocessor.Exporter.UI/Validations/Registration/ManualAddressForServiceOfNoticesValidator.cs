using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class ManualAddressForServiceOfNoticesValidator : AbstractValidator<ManualAddressForServiceOfNoticesViewModel>
{
   
    public ManualAddressForServiceOfNoticesValidator()
    {
        // Address
        Include(new AddressValidator());
    }
}
