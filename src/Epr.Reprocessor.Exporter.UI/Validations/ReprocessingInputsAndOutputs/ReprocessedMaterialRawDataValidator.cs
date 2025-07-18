namespace Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

using Epr.Reprocessor.Exporter.UI.Resources.Views.ReprocessingInputsAndOutputs;
using FluentValidation;

public class ReprocessedMaterialRawDataValidator : AbstractValidator<ReprocessedMaterialRawDataModel>
{
    public ReprocessedMaterialRawDataValidator()
    {
        RuleFor(x => x.MaterialOrProductName)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage(x => ValidationErrors.product_name_empty_error)
               .Matches("^[a-zA-Z ]+$")
               .WithMessage(x => ValidationErrors.product_name_non_alpha_error)
               .MaximumLength(50)
               .WithMessage(x => ValidationErrors.product_name_length_error);

        RuleFor(x => x.ReprocessedTonnes)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(x => ValidationErrors.product_tonnage_empty_error)
            .Must(ValidationHelpers.BeNullOrValidLongWithCommas)
            .WithMessage(x => ValidationErrors.product_tonnage_whole_number_error)
            .Must(ValidationHelpers.BeLongGreaterThanZero)
            .WithMessage(x => ValidationErrors.product_tonnage_lower_bound_error)
            .Must(ValidationHelpers.BeLongLessThanMaximum)
            .WithMessage(x => ValidationErrors.product_tonnage_upper_bound_error);
    }
}

internal static class ValidationHelpers
{
    public static bool BeNullOrValidIntegerWithCommas(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || value.TryConvertToInt(out _);
    }

    public static bool BeNullOrValidLongWithCommas(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || value.TryConvertToLong(out _);
    }

    public static bool BeLongGreaterThanZero(string? value)
    {
        if (value.TryConvertToLong(out long result))
        {
            return result is > 0;
        }

        return false;
    }

    public static bool BeLongLessThanMaximum(string? value)
    {
        if (value.TryConvertToLong(out long result))
        {
            return result is <= 10000000;
        }

        return false;
    }

    public static bool BeIntegerInValidRange(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (value.TryConvertToInt(out int result))
        {
            return result is >= 1 and <= 10000000;
        }

        return false;
    }
}