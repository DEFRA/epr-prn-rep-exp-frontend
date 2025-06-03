using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using FluentValidation;
using Address = Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials.Address;

namespace Epr.Reprocessor.Exporter.UI.Validations.Shared;

public class SelectedAddressValidator : AbstractValidator<SelectedAddressViewModel>
{
    public SelectedAddressValidator()
    {
        // SelectedIndex: Must not be null  
        RuleFor(x => x.SelectedIndex)
            .NotNull()
            .WithMessage(Address.ValidationMessage_AddressLine1_Required);

        // Postcode: Required and UK Postcode
        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage(Address.ValidationMessage_Postcode_Required);
    }
}
