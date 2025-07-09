namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation;
using System.Linq.Expressions;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {
        RuleFor(x => x)
               .Must(HaveAtLeastOneValue)
               .WithMessage("Enter a tonnage greater than 0 in at least one of reprocessing tonnage boxes")
               .WithErrorCode("1");

        ApplyTonnageRules(x => x.SentToOtherSiteTonnes);
        ApplyTonnageRules(x => x.ContaminantTonnes);
        ApplyTonnageRules(x => x.ProcessLossTonnes);

        RuleForEach(x => x.ReprocessedMaterialsRawData)
            .Where(RowHasAnyValue)
            .SetValidator(new ReprocessedMaterialRawDataValidator());
    }
    private void ApplyTonnageRules(Expression<Func<ReprocessedMaterialOutputSummaryModel, string?>> propertySelector)
    {
        RuleFor(propertySelector)
            .Cascade(CascadeMode.Stop)
            .Must(BeAValidTonnage).WithMessage("Enter tonnages in whole numbers, like 10")
            .Must(BeGreaterThanZero).WithMessage("Enter a tonnage greater than 0.")
            .Must(BeWithinRange).WithMessage("Weight must be 10,000,000 tonnes or less.")
            .WithErrorCode("1")
             .When(model => !string.IsNullOrWhiteSpace(propertySelector.Compile().Invoke(model)));
    }

    private static bool HaveAtLeastOneValue(ReprocessedMaterialOutputSummaryModel OutPutModel)
    {
        return !string.IsNullOrWhiteSpace(OutPutModel.SentToOtherSiteTonnes)
            || !string.IsNullOrWhiteSpace(OutPutModel.ContaminantTonnes)
            || !string.IsNullOrWhiteSpace(OutPutModel.ProcessLossTonnes);
    }

    // Helper methods
    private static bool BeAValidTonnage(string? input)
    {
        return int.TryParse(input, out _);
    }

    private static bool BeGreaterThanZero(string? input)
    {
        return decimal.TryParse(input, out var value) && value > 0;
    }

    private static bool BeWithinRange(string? input)
    {
       
        return int.TryParse(input, out var value) && value <= 10_000_000;
    }

    private static bool RowHasAnyValue(ReprocessedMaterialRawDataModel row)
    {
        return !string.IsNullOrWhiteSpace(row.MaterialOrProductName)
            || !string.IsNullOrWhiteSpace(row.ReprocessedTonnes);
    }
}