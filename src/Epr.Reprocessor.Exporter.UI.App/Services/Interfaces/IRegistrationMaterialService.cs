using Epr.Reprocessor.Exporter.UI.App.Domain;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

/// <summary>
/// Defines a contract to manage materials associated with a registration.
/// </summary>
public interface IRegistrationMaterialService
{
    /// <summary>
    /// Create a new registration material associated with the registration.
    /// </summary>
    /// <param name="registrationId">The unique ID of the registration associated with the registration material being created.</param>
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The created registration material dto.</returns>
    Task<Material> CreateAsync(Guid registrationId, CreateRegistrationMaterialDto request);

    /// <summary>
    /// Updates an existing registration material.
    /// </summary>
    /// <param name="registrationId">The unique ID of the registration associated with the registration material being updated.</param>
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The updated registration material dto.</returns>
    Task<Material> UpdateAsync(Guid registrationId, UpdateRegistrationMaterialDto request);

    /// <summary>
    /// Creates a registration material and exemption references for the given DTO.
    /// </summary>rms
    /// <param name="dto">The details relating to the exemptions.</param>
    /// <returns>The completed task.</returns>
    Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto);

    /// <summary>
    /// Gets all registration materials for a given registration.
    /// </summary>
    /// <param name="registrationId">The unique identifier for the overarching registration.</param>
    /// <returns>Collection of registration materials.</returns>
    Task<List<RegistrationMaterialDto>> GetAllRegistrationMaterialsAsync(Guid registrationId);
}