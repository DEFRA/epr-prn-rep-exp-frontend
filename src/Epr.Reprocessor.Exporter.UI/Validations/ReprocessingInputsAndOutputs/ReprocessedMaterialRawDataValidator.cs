using FluentValidation;

public class ReprocessedMaterialRawDataValidator : AbstractValidator<ReprocessedMaterialRawDataModel>
{
    public ReprocessedMaterialRawDataValidator()
    {
        RuleFor(x => x.MaterialOrProductName)
            .MaximumLength(100)
            .WithMessage("Products must be less than 100 characters.")
            .Matches("^[a-zA-Z]*$")
            .When(x => !string.IsNullOrEmpty(x.MaterialOrProductName))
            .WithMessage("Products must be written using letters.")
            .When(x => !string.IsNullOrEmpty(x.MaterialOrProductName));

        RuleFor(x => x.ReprocessedTonnes)
            .NotNull().WithMessage("Enter tonnages for your reprocessing outputs.")
            .When(x => !string.IsNullOrWhiteSpace(x.MaterialOrProductName));       

        RuleFor(x => x.ReprocessedTonnes.Value)
            .GreaterThan(0).WithMessage("Enter a tonnage greater than 0.")
             .InclusiveBetween(1, 10_000_000).WithMessage("Weight must be 10,000,000 tonnes or less.")
            .When(x => x.ReprocessedTonnes.HasValue && !string.IsNullOrWhiteSpace(x.MaterialOrProductName));

    }
}
