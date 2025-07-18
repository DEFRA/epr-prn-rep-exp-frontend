using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using FluentValidation;
using Address = Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials.Address;

namespace Epr.Reprocessor.Exporter.UI.Validations.ExorterJourney;

public class PostcodeForExporterSiteValidator : AbstractValidator<AddressSearchViewModel>
{
    public PostcodeForExporterSiteValidator()
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
