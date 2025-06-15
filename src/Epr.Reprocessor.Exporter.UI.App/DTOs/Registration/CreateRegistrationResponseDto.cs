namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a DTO for when creating a registration.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateRegistrationResponseDto
{
    /// <summary>
    /// The unique ID of the created registration.
    /// </summary>
    public Guid Id { get; set; }
}