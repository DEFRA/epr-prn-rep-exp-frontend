namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using FluentValidation;

public class ReprocessingOutputModelValidator : AbstractValidator<ReprocessedMaterialOutputSummaryModel>
{
    public ReprocessingOutputModelValidator()
    {
        RuleFor(x => x.SentToOtherSiteTonnes)
            .NotEmpty().WithMessage("Enter tonnages for your reprocessing outputs.")
            .Must(BeAValidTonnage).WithMessage("Enter a valid number.")
            .Must(BeGreaterThanZero).WithMessage("Enter a tonnage greater than 0.")
            .Must(BeWithinRange).WithMessage("Weight must be 10,000,000 tonnes or less.");

        RuleFor(x => x.ContaminantTonnes)
            .NotEmpty().WithMessage("Enter tonnages for your reprocessing outputs.")
            .Must(BeAValidTonnage).WithMessage("Enter a valid number.")
            .Must(BeGreaterThanZero).WithMessage("Enter a tonnage greater than 0.")
            .Must(BeWithinRange).WithMessage("Weight must be 10,000,000 tonnes or less.");

        RuleFor(x => x.ProcessLossTonnes)
            .NotEmpty().WithMessage("Enter tonnages for your reprocessing outputs.")
            .Must(BeAValidTonnage).WithMessage("Enter a valid number.")
            .Must(BeGreaterThanZero).WithMessage("Enter a tonnage greater than 0.")
            .Must(BeWithinRange).WithMessage("Weight must be 10,000,000 tonnes or less.");

        RuleForEach(x => x.ReprocessedMaterialsRawData)
            .Where(RowHasAnyValue)
            .SetValidator(new ReprocessedMaterialRawDataValidator());
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
            || !string.IsNullOrWhiteSpace(row.ReprocessedTonnes.ToString());
    }
}
