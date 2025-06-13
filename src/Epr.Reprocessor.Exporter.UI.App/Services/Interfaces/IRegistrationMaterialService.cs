using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

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
    Task<Material> CreateAsync(int registrationId, CreateRegistrationMaterialDto request);

    /// <summary>
    /// Updates an existing registration material.
    /// </summary>
    /// <param name="registrationId">The unique ID of the registration associated with the registration material being updated.</param>
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The updated registration material dto.</returns>
    Task<Material> UpdateAsync(int registrationId, UpdateRegistrationMaterialDto request);

    /// <summary>
    /// Creates exemption references for the given DTO.
    /// </summary>
    /// <param name="dto">The details relating to the exemptions.</param>
    /// <returns>The completed task.</returns>
    Task CreateExemptionReferences(CreateExemptionReferencesDto dto);

    /// <summary>
    /// Creates a registration material with the specified registration ID and material.
    /// </summary>
    /// <param name="registrationId"></param>
    /// <param name="material"></param>
    /// <returns>registration material id</returns>
    Task<int> CreateRegistrationMaterial(int registrationId, string material);
}