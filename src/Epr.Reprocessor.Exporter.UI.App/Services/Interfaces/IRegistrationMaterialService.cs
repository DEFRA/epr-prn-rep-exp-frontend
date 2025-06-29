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
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The created registration material dto.</returns>
    Task<RegistrationMaterial?> CreateAsync(CreateRegistrationMaterialDto request);

    /// <summary>
    /// Updates an existing registration material.
    /// </summary>
    /// <param name="registrationId">The unique ID of the registration associated with the registration material being updated.</param>
    /// <param name="request">The request associated with this call.</param>
    /// <returns>The updated registration material dto.</returns>
    Task<RegistrationMaterial> UpdateAsync(Guid registrationId, UpdateRegistrationMaterialDto request);

    /// <summary>
    /// Gets all registration materials for a given registration.
    /// </summary>
    /// <param name="registrationId">The unique identifier for the overarching registration.</param>
    /// <returns>Collection of registration materials.</returns>
    Task<List<RegistrationMaterial>> GetAllRegistrationMaterialsAsync(Guid registrationId);
    Task CreateExemptionReferences(CreateExemptionReferencesDto dto);

    Task UpdateRegistrationMaterialPermitsAsync(Guid id, UpdateRegistrationMaterialPermitsDto request);

    Task UpdateRegistrationMaterialPermitCapacityAsync(Guid id, UpdateRegistrationMaterialPermitCapacityDto request);

    Task<List<MaterialsPermitTypeDto>> GetMaterialsPermitTypesAsync();

    /// <summary>
    /// Deletes an existing registration material with the specified ID. Note that this is a hard delete.
    /// </summary>
    /// <param name="registrationMaterialId">The unique identifier of the registration material to delete.</param>
    /// <returns>The completed task.</returns>
    Task DeleteAsync(Guid registrationMaterialId);

    /// <summary>
    /// Updates the maximum weight that the site can process ofr a given material.
    /// </summary>
    /// <param name="registrationMaterialId">The unique identifier for the registration material.</param>
    /// <param name="weightInTonnes">The weight in tonnes to set.</param>
    /// <param name="period">The period within which the weight is applicable.</param>
    /// <returns>The completed task.</returns>
    Task UpdateMaximumWeightCapableForReprocessingAsync(
        Guid registrationMaterialId,
        decimal weightInTonnes,
        PeriodDuration period);
}