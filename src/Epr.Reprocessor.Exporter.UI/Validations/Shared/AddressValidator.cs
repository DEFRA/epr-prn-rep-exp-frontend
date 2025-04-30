using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Shared;

public class AddressValidator : AbstractValidator<AddressViewModel>
{
    public AddressValidator()
    {
        // AddressLine1: Required and MaxLength
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage(Address.ValidationMessage_AddressLine1_Required)
            .MaximumLength(MaxLengths.AddressLine1)
            .WithMessage(Address.ValidationMessage_AddressLine1_MaxLength);

        // AddressLine2: MaxLength 
        RuleFor(x => x.AddressLine2)
            .MaximumLength(MaxLengths.AddressLine2)
            .WithMessage(Address.ValidationMessage_AddressLine2_MaxLength);

        // TownOrCity: MaxLength 
        RuleFor(x => x.TownOrCity)
            .NotEmpty()
            .WithMessage(Address.ValidationMessage_TownOrCity_Required)
            .MaximumLength(MaxLengths.TownOrCity)
            .WithMessage(Address.ValidationMessage_TownOrCity_MaxLength);

        // County: MaxLength 
        RuleFor(x => x.County)
            .MaximumLength(MaxLengths.County)
            .WithMessage(Address.ValidationMessage_County_MaxLength);

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
