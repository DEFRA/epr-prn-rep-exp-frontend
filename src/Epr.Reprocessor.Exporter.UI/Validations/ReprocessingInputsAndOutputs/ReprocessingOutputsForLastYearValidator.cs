namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using Epr.Reprocessor.Exporter.UI.Resources.Views.ReprocessingInputsAndOutputs;
using FluentValidation;
using System.Linq.Expressions;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOneValue).WithMessage(x => ReprocessingOutputsForLastYear.tonnage_none_filled_error);

        When(x => !string.IsNullOrWhiteSpace(x.SentToOtherSiteTonnes), () =>
        {
            ApplyTonnageRules(x => x.SentToOtherSiteTonnes);
        });

        When(x => !string.IsNullOrWhiteSpace(x.ContaminantTonnes), () =>
        {
            ApplyTonnageRules(x => x.ContaminantTonnes);
        });

        When(x => !string.IsNullOrWhiteSpace(x.ProcessLossTonnes), () =>
        {
            ApplyTonnageRules(x => x.ProcessLossTonnes);
        });

        RuleForEach(x => x.ReprocessedMaterialsRawData)
            .Where(RowHasAnyValue)
            .SetValidator(new ReprocessedMaterialRawDataValidator());
    }

    private void ApplyTonnageRules(Expression<Func<ReprocessedMaterialOutputSummaryModel, string?>> propertySelector)
    {
        RuleFor(propertySelector)
            .Cascade(CascadeMode.Stop)
            .Must(BeAValidTonnage).WithMessage(x => ReprocessingOutputsForLastYear.tonnage_whole_number_error)
            .Must(BeGreaterThanZero).WithMessage(x => ReprocessingOutputsForLastYear.tonnage_lower_bound_error)
            .Must(BeWithinRange).WithMessage(x => ReprocessingOutputsForLastYear.tonnage_upper_bound_error);
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
        return long.TryParse(input, out _);
    }

    private static bool BeGreaterThanZero(string? input)
    {
        return long.TryParse(input, out var value) && value > 0;
    }

    private static bool BeWithinRange(string? input)
    {
        return long.TryParse(input, out var value) && value <= 10_000_000;
    }

    private static bool RowHasAnyValue(ReprocessedMaterialRawDataModel row)
    {
        return !string.IsNullOrWhiteSpace(row.MaterialOrProductName)
            || !string.IsNullOrWhiteSpace(row.ReprocessedTonnes);
    }
}