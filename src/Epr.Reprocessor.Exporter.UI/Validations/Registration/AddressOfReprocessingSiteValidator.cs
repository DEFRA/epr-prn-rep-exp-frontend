using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class AddressOfReprocessingSiteValidator : AbstractValidator<AddressOfReprocessingSiteViewModel>
{
    public AddressOfReprocessingSiteValidator()
    {
        RuleFor(x => x.SelectedOption)
            .NotEmpty()
            .WithMessage(AddressOfReprocessingSite.SelectAnOptionErrorMessage);
    }
}
