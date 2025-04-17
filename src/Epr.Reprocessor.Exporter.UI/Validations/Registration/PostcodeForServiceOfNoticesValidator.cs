using System.Text.RegularExpressions;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class PostcodeForServiceOfNoticesValidator : AbstractValidator<PostcodeForServiceOfNoticesViewModel>
{
    private static readonly Regex _ukPostcodeRegex = new(
        @"^(GIR 0AA|[A-PR-UWYZ][A-HK-Y0-9][A-HJKS-UW0-9]?[0-9][ABD-HJLNP-UW-Z]{2})$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
    
    public PostcodeForServiceOfNoticesValidator()
    {
        // Postcode: Required
        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage(PostcodeForServiceOfNotices.ValidationMessage_Postcode_Required);

        // Postcode: Must be valid UK Postcode
        RuleFor(x => x.Postcode)
            .Must(BeValidUKPostcode)
            .WithMessage(PostcodeForServiceOfNotices.ValidationMessage_Postcode_Invalid)
            .When(x => !string.IsNullOrWhiteSpace(x.Postcode));
    }

    private static bool BeValidUKPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode)) return false;

        postcode = postcode.ToUpper().Replace(" ", string.Empty);

        return _ukPostcodeRegex.IsMatch(postcode);
    }
}
