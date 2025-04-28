using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class ManualAddressForReprocessingSiteValidator : AbstractValidator<ManualAddressForReprocessingSiteViewModel>
{
    private const int MinNoOfDigits = 4;
    private const int MaxNoOfDigits = 10;

    public ManualAddressForReprocessingSiteValidator()
    {
        // Address
        Include(new AddressValidator());

        // SiteGridReference: Required, Minimum length, Maximum length and special characters 
        RuleFor(x => x.SiteGridReference)
            .NotEmpty()
            .WithMessage(ManualAddressForReprocessingSite.ValidationMessage_SiteGridReference_Required);

        When(x => !string.IsNullOrWhiteSpace(x.SiteGridReference), () =>
        {
            // Minimum Length of trailing digits
            RuleFor(x => x.SiteGridReference)
                .Must(sgr => RegexHelper.CountEndingDigits(sgr) >= MinNoOfDigits)
                .WithMessage(ManualAddressForReprocessingSite.ValidationMessage_SiteGridReference_MinLength);

            // Maximum Length of trailing digits
            RuleFor(x => x.SiteGridReference)
                .Must(sgr => RegexHelper.CountEndingDigits(sgr) <= MaxNoOfDigits)
                .WithMessage(ManualAddressForReprocessingSite.ValidationMessage_SiteGridReference_MaxLength);

            // Alpha Numeric - No Special Characters
            RuleFor(x => x.SiteGridReference)
                .Must(RegexHelper.IsAlphaNumeric)
                .WithMessage(ManualAddressForReprocessingSite.ValidationMessage_SiteGridReference_SpecialCharacters);
        });
    }
}
