using System.Threading.Tasks;
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
    /// Creates a registration material and exemption references for the given DTO.
    /// </summary>
    /// <param name="dto">The details relating to the exemptions.</param>
    /// <returns>The completed task.</returns>
    Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto);

    /// <summary>
    /// Update registration material permits 
    /// </summary>
    /// <param name="externalId">The external Id for registration material</param>
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The completed task.</returns>
    Task UpdateRegistrationMaterialPermitsAsync(Guid externalId, UpdateRegistrationMaterialPermitsDto request);

    /// <summary>
    /// Get list of material permit types
    /// </summary>
    /// <returns>List of permit types</returns>
    Task<List<MaterialsPermitTypeDto>> GetMaterialsPermitTypesAsync();
}