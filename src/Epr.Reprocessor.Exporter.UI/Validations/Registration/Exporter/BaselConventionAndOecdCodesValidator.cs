using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;
public class BaselConventionAndOecdCodesValidator : AbstractValidator<BaselConventionAndOecdCodesViewModel>
{
    public BaselConventionAndOecdCodesValidator()
    {
        RuleFor(x => x.OecdCodes)
            .Must(codes => codes.Exists(c => !string.IsNullOrWhiteSpace(c.CodeName)))
            .WithMessage(BaselConventionAndOecdCodes.ValidationMessage_OecdCodes_MustEnteredAtleastOneCode
            );
    }
}