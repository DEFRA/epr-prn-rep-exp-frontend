namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;
using FluentValidation;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {


        RuleFor(x => x.SentToOtherSiteTonnes)
        .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.SentToOtherSiteTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
            .When(x => x.SentToOtherSiteTonnes.HasValue);


        RuleFor(x => x.ContaminantTonnes)
      .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ContaminantTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
            .When(x => x.ContaminantTonnes.HasValue);

        RuleFor(x => x.ProcessLossTonnes)
      .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ProcessLossTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
            .When(x => x.ProcessLossTonnes.HasValue);

        RuleForEach(x => x.ReprocessedMaterialsRawData)
            .SetValidator(new ReprocessedMaterialRawDataValidator());
    }
}
