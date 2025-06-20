namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Represents a response dto for creating a registration material.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateRegistrationMaterialResponseDto
{
    /// <summary>
    /// Unique identifier for the registration material.
    /// </summary>
    public Guid Id { get; set; }
}