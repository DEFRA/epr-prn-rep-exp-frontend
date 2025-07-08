namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation;

public class ReprocessedMaterialRawDataValidator : AbstractValidator<ReprocessedMaterialRawDataModel>
{
    public ReprocessedMaterialRawDataValidator()
    {
         RuleFor(x => x.MaterialOrProductName)
                .NotEmpty()
                .WithMessage("Enter the Products name .")
                .Matches("^[a-zA-Z ]+$")
                .WithMessage("Products must be written using letters.")
                .MaximumLength(50)
                .WithMessage("Products must be less than 100 characters.");

        RuleFor(x => x.ReprocessedTonnes)
            .NotEmpty()
            .WithMessage("Enter tonnages for your reprocessing outputs.")
            .Must(ValidationHelpers.BeNullOrValidIntegerWithCommas)
            .WithMessage("Enter tonnages in whole numbers, like 10")
            .Must(ValidationHelpers.BeIntegerInValidRange)
            .WithMessage("Weight must be 10,000,000 tonnes or less.");
    }
}
