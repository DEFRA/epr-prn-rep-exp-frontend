using System.Text.RegularExpressions;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class ManualAddressForServiceOfNoticesValidator : AbstractValidator<ManualAddressForServiceOfNoticesViewModel>
{
    private static readonly Regex _ukPostcodeRegex = new(
        @"^(GIR 0AA|[A-PR-UWYZ][A-HK-Y0-9][A-HJKS-UW0-9]?[0-9][ABD-HJLNP-UW-Z]{2})$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(250));
    
    public ManualAddressForServiceOfNoticesValidator()
    {
        // AddressLine1: Required and MaxLength
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_AddressLine1_Required)
            .MaximumLength(100)
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_AddressLine1_MaxLength);

        // AddressLine2: MaxLength 
        RuleFor(x => x.AddressLine2)
            .MaximumLength(100)
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_AddressLine2_MaxLength);

        // TownOrCity: MaxLength 
        RuleFor(x => x.TownOrCity)
            .NotEmpty()
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_TownOrCity_Required)
            .MaximumLength(70)
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_TownOrCity_MaxLength);

        // County: MaxLength 
        RuleFor(x => x.County)
            .MaximumLength(50)
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_County_MaxLength);

        // Postcode: Required and UK Postcode
        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_Postcode_Required);

        RuleFor(x => x.Postcode)
            .Must(BeValidUKPostcode)
            .WithMessage(ManualAddressForServiceOfNotices.ValidationMessage_Postcode_Invalid)
            .When(x => !string.IsNullOrWhiteSpace(x.Postcode));
    }

    private static bool BeValidUKPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode)) return false;

        postcode = postcode.ToUpper().Replace(" ", string.Empty);

        return _ukPostcodeRegex.IsMatch(postcode);
    }
}
