using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using FluentValidation;
using Address = Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials.Address;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class PostcodeForReprocessingSiteValidator : AbstractValidator<PostcodeOfReprocessingSiteViewModel>
{
    public PostcodeForReprocessingSiteValidator()
    {
        // Postcode: Required and UK Postcode
        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage(Address.ValidationMessage_Postcode_Required);

        RuleFor(x => x.Postcode)
            .Must(RegexHelper.ValidateUKPostcode)
            .WithMessage(Address.ValidationMessage_Postcode_Invalid)
            .When(x => !string.IsNullOrWhiteSpace(x.Postcode));
    }
}
