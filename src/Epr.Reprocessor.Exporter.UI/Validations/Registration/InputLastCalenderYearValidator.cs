using FluentValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class InputLastCalenderYearValidator : AbstractValidator<InputLastCalenderYearViewModel>
{
    public InputLastCalenderYearValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOnePositiveValue)
            .WithMessage("Enter a positive whole number in at least one of the tonnage boxes listed");

    }

    private static bool HaveAtLeastOnePositiveValue(InputLastCalenderYearViewModel viewModel)
    {
        return (viewModel.UkPackagingWaste ?? 0) > 0
            || (viewModel.NonUkPackagingWaste ?? 0) > 0
            || (viewModel.NonPackagingWaste ?? 0) > 0;
    }
}
