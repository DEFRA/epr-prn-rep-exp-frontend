using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class UpdateRegistrationMaterialPermitCapacityCommandValidator : AbstractValidator<UpdateRegistrationMaterialPermitCapacityDto>
{
    public UpdateRegistrationMaterialPermitCapacityCommandValidator()
    {
        // PermitTypeId
        RuleFor(x => x.PermitTypeId)
            .NotEmpty()
            .WithMessage("PermitTypeId is required");

        // CapacityInTonnes
        RuleFor(x => x.CapacityInTonnes)
            .NotNull()
            .WithMessage("Weight must be a number greater than 0");

        RuleFor(x => x.CapacityInTonnes)
            .Must(x => x != 0)
            .WithMessage("Weight must be a number greater than 0");

        RuleFor(x => x.CapacityInTonnes)
            .Must(x => x.GetValueOrDefault() < 10000000)
            .WithMessage("Weight must be a number less than 10,000,000")
            .When(x => x.CapacityInTonnes.GetValueOrDefault() > 0);

        // PeriodId
        RuleFor(x => x.PeriodId)
            .NotEmpty()
            .WithMessage("PeriodId is required");
    }
}
