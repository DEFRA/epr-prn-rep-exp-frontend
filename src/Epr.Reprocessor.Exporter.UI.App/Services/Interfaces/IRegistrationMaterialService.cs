using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums;

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
    /// Updates the status of a registration task associated with a registration material.
    /// </summary>
    /// <param name="registrationMaterialId">The unique identifier for the material.</param>
    /// <param name="taskName">The task name to set the status of.</param>
    /// <param name="status">The status to set the task as.</param>
    /// <returns>The completed task.</returns>
    Task UpdateTaskStatusAsync(Guid registrationMaterialId, TaskType taskName, ApplicantRegistrationTaskStatus status);
}