
using System;
using System.Globalization;
using System.Linq.Expressions;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration
{
    public class InputsForLastCalendarYearValidator : AbstractValidator<InputsForLastCalendarYearViewModel>
    {
        public InputsForLastCalendarYearValidator()
        {
            RuleFor(x => x)
                .Must(HaveAtLeastOneValue)
                .WithMessage("Enter a tonnage greater than 0 in at least one of the waste tonnage boxes");

            ApplyTonnageRules(x => x.UkPackagingWaste);
            ApplyTonnageRules(x => x.NonUkPackagingWaste);
            ApplyTonnageRules(x => x.NonPackagingWaste);

            RuleForEach(x => x.RawMaterials)
                .Where(RowHasAnyValue)
                .SetValidator(new RawMaterialRowValidator());
        }

        private void ApplyTonnageRules(Expression<Func<InputsForLastCalendarYearViewModel, string?>> propertySelector)
        {
            RuleFor(propertySelector)
                .Cascade(CascadeMode.Stop)
                .Must(ValidationHelpers.BeNullOrValidIntegerWithCommas)
                .WithMessage("Enter tonnages in whole numbers, like 10")
                .Must(ValidationHelpers.BeIntegerInValidRange)
                .WithMessage("Weight must be more than 0 and not greater than 10,000,000 tonnes");
        }

        private static bool HaveAtLeastOneValue(InputsForLastCalendarYearViewModel viewModel)
        {
            return !string.IsNullOrWhiteSpace(viewModel.UkPackagingWaste)
                || !string.IsNullOrWhiteSpace(viewModel.NonUkPackagingWaste)
                || !string.IsNullOrWhiteSpace(viewModel.NonPackagingWaste);
        }

        private static bool RowHasAnyValue(RawMaterialRowViewModel row)
        {
            return !string.IsNullOrWhiteSpace(row.RawMaterialName)
                || !string.IsNullOrWhiteSpace(row.Tonnes);
        }
    }

    public class RawMaterialRowValidator : AbstractValidator<RawMaterialRowViewModel>
    {
        public RawMaterialRowValidator()
        {
            RuleFor(x => x.RawMaterialName)
                .NotEmpty()
                .WithMessage("Enter the name of a raw material")
                .Matches("^[a-zA-Z ]+$")
                .WithMessage("Raw materials must be written using letters")
                .MaximumLength(50)
                .WithMessage("Raw materials must be less than 50 characters");

            RuleFor(x => x.Tonnes)
                .NotEmpty()
                .WithMessage("Enter a tonnage for the raw material")
                .Must(ValidationHelpers.BeNullOrValidIntegerWithCommas)
                .WithMessage("Enter tonnages in whole numbers, like 10")
                .Must(ValidationHelpers.BeIntegerInValidRange)
                .WithMessage("Weight must be more than 0 and not greater than 10,000,000 tonnes");
        }
    }

    internal static class ValidationHelpers
    {
        public static bool BeNullOrValidIntegerWithCommas(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ||
                   int.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        public static bool BeIntegerInValidRange(string? value)
        {
        if (string.IsNullOrWhiteSpace(value))
            return true;

            if (int.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result is >= 1 and <= 10000000;
            }

           return false;
        }
    }

}
