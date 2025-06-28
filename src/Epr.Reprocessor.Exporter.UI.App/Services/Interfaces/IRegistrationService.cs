namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

/// <summary>
/// Defines a contract to manage a registration.
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// Creates a new registration.
    /// </summary>
    /// <param name="request">The request associated with the call.</param>
    /// <returns>A registration ID.</returns>
    Task<CreateRegistrationResponseDto> CreateAsync(CreateRegistrationDto request);

    /// <summary>
    /// Retrieves a registration by its ID.
    /// </summary>
    /// <param name="registrationId">The registration to fetch with.</param>
    /// <returns>An object containing the details of the registration, if found.</returns>
    Task<RegistrationDto?> GetAsync(Guid registrationId);

    /// <summary>
    /// Retrieves a registration by its application type ID and organisation ID.
    /// </summary>
    /// <param name="applicationTypeId">The ID of the application type to search by.</param>
    /// <param name="organisationId">The ID of the organisation associated with the user.</param>
    /// <returns>An object containing the details of the registration, if found.</returns>
    Task<RegistrationDto?> GetByOrganisationAsync(int applicationTypeId, Guid organisationId);

    /// <summary>
    /// Updates the registration with the provided request data.
    /// </summary>
    /// <param name="registrationId">The unique identifier for the registration that is being updated.</param>
    /// <param name="request">The request associated with the call.</param>
    Task UpdateAsync(Guid registrationId, UpdateRegistrationRequestDto request);

    /// <summary>
    /// Updates the registration site address for a given registration ID.
    /// </summary>
    /// <param name="registrationId">The registration to fetch with.</param>
    /// <param name="request">The request associated with the call.</param>
    /// <returns>The completed task.</returns>
    Task UpdateRegistrationSiteAddressAsync(Guid registrationId, UpdateRegistrationSiteAddressDto request);

    /// <summary>
    /// Updates the status of a registration task for a given registration ID.
    /// </summary>
    /// <param name="registrationId">The registration to fetch with.</param>
    /// <param name="request">The request associated with the call.</param>
    /// <returns>The completed task.</returns>
    Task UpdateRegistrationTaskStatusAsync(Guid registrationId, UpdateRegistrationTaskStatusDto request);

    /// <summary>
    /// Retrieves all registrations and accreditations for a given organisation ID.
    /// </summary>
    /// <param name="organisationId">The ID of the organisation associated with the user.</param>
    /// <returns>A collection of registration objects.</returns>
    Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid? organisationId);
    Task<IEnumerable<string>> GetCountries();
}