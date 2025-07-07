namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {


        RuleFor(x => x.SentToOtherSiteTonnes)
        .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.SentToOtherSiteTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
              .InclusiveBetween(1, 10_000_000).WithMessage("Weight must be 10,000,000 tonnes or less.")
            .When(x => x.SentToOtherSiteTonnes.HasValue);


        RuleFor(x => x.ContaminantTonnes)
      .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ContaminantTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
             .InclusiveBetween(1, 10_000_000).WithMessage("Weight must be 10,000,000 tonnes or less.")
            .When(x => x.ContaminantTonnes.HasValue);

        RuleFor(x => x.ProcessLossTonnes)
      .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.");

        RuleFor(x => x.ProcessLossTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
             .InclusiveBetween(1, 10_000_000).WithMessage("Weight must be 10,000,000 tonnes or less.")
            .When(x => x.ProcessLossTonnes.HasValue);

        RuleForEach(x => x.ReprocessedMaterialsRawData)
               .Where(RowHasAnyValue)
               .SetValidator(new ReprocessedMaterialRawDataValidator());

    }
    private static bool RowHasAnyValue(ReprocessedMaterialRawDataModel row)
    {
        return !string.IsNullOrWhiteSpace(row.MaterialOrProductName)
            || !string.IsNullOrWhiteSpace(row.ReprocessedTonnes.ToString());
    }
}
