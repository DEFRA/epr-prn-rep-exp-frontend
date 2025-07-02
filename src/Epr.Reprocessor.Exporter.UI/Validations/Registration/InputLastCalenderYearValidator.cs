using System.Linq.Expressions;
using FluentValidation;

public class InputLastCalenderYearValidator : AbstractValidator<InputsForLastCalendarYearViewModel>
{
    public InputLastCalenderYearValidator()
    {
        // Combined waste field validation
        RuleFor(x => x)
            .Must(HaveAtLeastOnePositiveValue)
            .WithMessage("Enter a tonnage greater than 0 in at least one of the tonnage boxes listed");
        
        //Individual wate field validation
        AddTonnageValidationRule(x => x.UkPackagingWaste);
        AddTonnageValidationRule(x => x.NonUkPackagingWaste);
        AddTonnageValidationRule(x => x.NonPackagingWaste);

        // Validate only non-empty RawMaterial rows
        RuleForEach(x => x.RawMaterials)
            .Where(RowHasAnyValue)
            .SetValidator(new RawMaterialRowValidator());
    }

    private void AddTonnageValidationRule(Expression<Func<InputsForLastCalendarYearViewModel, int?>> selector)
    {
        RuleFor(selector)
            .Must(BeWholeNumberOrNull)
            .WithMessage("Enter tonnages in whole numbers and greater than 0, like 10")
            .InclusiveBetween(1, 10000000)
            .When(model => selector.Compile().Invoke(model).HasValue)
            .WithMessage("Weight must be greater than 0 and 10,000,000 tonnes or less");
    }

    private static bool HaveAtLeastOnePositiveValue(InputsForLastCalendarYearViewModel viewModel)
    {
        return (viewModel.UkPackagingWaste ?? 0) > 0
            || (viewModel.NonUkPackagingWaste ?? 0) > 0
            || (viewModel.NonPackagingWaste ?? 0) > 0;
    }

    private static bool BeWholeNumberOrNull(int? value)
    {
        return !value.HasValue || value >= 0;
    }

    private static bool RowHasAnyValue(RawMaterialRowViewModel row)
    {
        return !string.IsNullOrWhiteSpace(row.RawMaterialName)
            || row.Tonnes.HasValue;
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
            .NotNull()
            .WithMessage("Enter a tonnage for the raw material")
            .Must(BeWholeNumberOrNull)
            .WithMessage("Enter tonnages in whole numbers, like 10");
    }
    private static bool BeWholeNumberOrNull(int? value)
    {
        return !value.HasValue || value >= 0;
    }
}
