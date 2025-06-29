namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// Represents the view model for applying for registration.
/// </summary>
public class ApplyForRegistrationViewModel
{
    /// <summary>
    /// Gets or sets the type of application for registration.
    /// Defaults to <see cref="ApplicationType.Unspecified"/>.
    /// </summary>
    public ApplicationType ApplicationType { get; set; } = ApplicationType.Unspecified;
}
