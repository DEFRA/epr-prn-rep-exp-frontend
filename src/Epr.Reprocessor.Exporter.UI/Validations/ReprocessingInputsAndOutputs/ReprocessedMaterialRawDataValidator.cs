using FluentValidation;

public class ReprocessedMaterialRawDataValidator : AbstractValidator<ReprocessedMaterialRawDataModel>
{
    public ReprocessedMaterialRawDataValidator()
    {
        RuleFor(x => x.MaterialOrProductName)
            .MaximumLength(100)
            .WithMessage("Products must be less than 100 characters.")
            .Matches("^[a-zA-Z]*$")
            .When(x => !string.IsNullOrWhiteSpace(x.MaterialOrProductName))
            .WithMessage("Products must be written using letters.");

        RuleFor(x => x.ReprocessedTonnes)
            .GreaterThan(0)
            .WithMessage("Tonnage is required and must be greater than 0 when Products is provided.")
            .When(x => !string.IsNullOrWhiteSpace(x.MaterialOrProductName));
    }
}
