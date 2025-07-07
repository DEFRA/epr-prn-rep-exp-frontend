using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

/// <summary>
/// Validator for the ApplyForRegistrationViewModel to ensure the application type is valid.
/// </summary>
public class ApplyForRegistrationViewModelValidator : AbstractValidator<ApplyForRegistrationViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplyForRegistrationViewModelValidator"/> class.
    /// </summary>
    public ApplyForRegistrationViewModelValidator()
    {
        RuleFor(model => model.ApplicationType)
            .Must(IsValidApplicationType)
            .WithMessage(ApplyForRegistration.ValidationMessage_SelectApplicationType);
    }

    /// <summary>
    /// Validates that the provided <see cref="ApplicationType"/> is a defined enum value and not 'Unspecified'.
    /// </summary>
    /// <param name="applicationType">The <see cref="ApplicationType"/> to validate.</param>
    /// <returns>
    /// <c>true</c> if the application type is valid (i.e., it is a defined enum value and not 'Unspecified');
    /// otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidApplicationType(ApplicationType applicationType)
    {
        return Enum.IsDefined(typeof(ApplicationType), applicationType) &&
               applicationType != ApplicationType.Unspecified;
    }
}
