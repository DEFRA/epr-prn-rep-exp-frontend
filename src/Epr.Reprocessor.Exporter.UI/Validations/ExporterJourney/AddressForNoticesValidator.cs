using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using FluentValidation;
using AddressForNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.AddressForNoticesViewModel;

namespace Epr.Reprocessor.Exporter.UI.Validations.ExporterJourney;

[ExcludeFromCodeCoverage]
public class AddressForNoticesValidator : AbstractValidator<AddressForNoticesViewModel>
{
    public AddressForNoticesValidator()
    {
        // Address
        Include(new AddressValidator());
    }
}
