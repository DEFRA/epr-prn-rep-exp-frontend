using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;
using FluentValidation;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {
        RuleFor(x => x.SentToOtherSiteTonnes)
            .GreaterThan(0)
            .WithMessage("Enter a tonnage greater than 0.")
            .NotNull()
            .WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ContaminantTonnes)
            .GreaterThan(0)
            .WithMessage("Enter a tonnage greater than 0.")
            .NotNull()
            .WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ProcessLossTonnes)
            .GreaterThan(0)
            .WithMessage("Enter a tonnage greater than 0.")
            .NotNull()
            .WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x)
            .Must(model => model.TotalOutputTonnes <= model.TotalInputTonnes)
            .WithMessage("Total output tonnage cannot exceed total input tonnage.");
    }
}
