using Epr.Reprocessor.Exporter.UI.App.Domain;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines the request to create a registration material.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateRegistrationMaterialDto
{
    /// <summary>
    /// The material to update.
    /// </summary>
    public RegistrationMaterial RegistrationMaterial { get; set; } = null!;
}