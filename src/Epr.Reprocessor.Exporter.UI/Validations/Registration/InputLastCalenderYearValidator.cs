using System.Linq.Expressions;
using FluentValidation;

public class InputLastCalenderYearValidator : AbstractValidator<InputsForLastCalendarYearViewModel>
{
    public InputLastCalenderYearValidator()
    {
        // Combined waste field validation
        RuleFor(x => x)
            .Must(HaveAtLeastOnePositiveValue)
            .WithMessage("Enter a value in at least one of the tonnage boxes listed");
        
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
            .WithMessage("Tonnage must be a whole number greater than 0, like 10")
            .InclusiveBetween(1, 10000000)
            .When(model => selector.Compile().Invoke(model).HasValue)
            .WithMessage("Weight must be more than 0 and no greater than 10,000,000 tonnes.");
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
            .WithMessage("Raw material names must contain letters only")
            .MaximumLength(50)
            .WithMessage("Raw material name must be less than 50 characters");

        RuleFor(x => x.Tonnes)
            .NotNull()
            .WithMessage("Enter a tonnage value for the raw material")
            .Must(BeWholeNumberOrNull)
            .WithMessage("Tonnage must be a whole number greater than 0, like 10")
            .InclusiveBetween(1, 10000000)
            .WithMessage("Weight must be more than 0 and no greater than 10,000,000 tonnes.");
    }
    private static bool BeWholeNumberOrNull(int? value)
    {
        return !value.HasValue || value >= 0;
    }
}
