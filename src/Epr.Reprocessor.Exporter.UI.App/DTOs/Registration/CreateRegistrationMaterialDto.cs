namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines the request to create a registration material.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateRegistrationMaterialDto
{
    /// <summary>
    /// The unique identifier for the over arching registration.
    /// </summary>
    public required Guid RegistrationId { get; set; }

    /// <summary>
    /// The material to create.
    /// </summary>
    public required string Material { get; set; } = null!;
}