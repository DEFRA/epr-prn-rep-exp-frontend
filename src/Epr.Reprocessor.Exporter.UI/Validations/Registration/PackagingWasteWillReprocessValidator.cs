using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class PackagingWasteWillReprocessValidator : AbstractValidator<PackagingWasteWillReprocessViewModel>
{
    public PackagingWasteWillReprocessValidator()
    {
        RuleFor(x => x.SelectedRegistrationMaterials)
            .NotNull()
            .Must(list => list.Count > 0)
            .WithMessage("Select the packaging waste you are reprocessing");
    }
}
