using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Defines a contract to manage a registration.
/// </summary>
/// <remarks>If you need to manage registration materials then use the <see cref="RegistrationMaterialService"/> as this handles materials.</remarks>
/// <param name="client">The underlying http client that will call the facade.</param>
/// <param name="logger">The logger to log to.</param>
[ExcludeFromCodeCoverage]
public class RegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    /// <inheritdoc/>
    public async Task<CreateRegistrationResponseDto> CreateAsync(CreateRegistrationDto request)
    {
        try
        {
            var uri = Endpoints.Registration.CreateRegistration;

            var result = await client.SendPostRequest(uri, request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await result.Content.ReadFromJsonAsync<CreateRegistrationResponseDto>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RegistrationDto?> GetAsync(Guid registrationId)
    {
        try
        {
            var result = await client.SendGetRequest(string.Format(Endpoints.Registration.GetRegistration, registrationId.ToString()));
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<RegistrationDto>(options);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RegistrationDto?> GetByOrganisationAsync(int applicationTypeId, Guid organisationId)
    {
        try
        {
            var result = await client.SendGetRequest(string .Format(Endpoints.Registration.GetByOrganisation, applicationTypeId, organisationId));
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<RegistrationDto>(options);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration by organisation");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Guid registrationId, UpdateRegistrationRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.Registration.UpdateRegistration, registrationId), request);
            result.EnsureSuccessStatusCode();

        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid? organisationId)
    {
        if (organisationId == Guid.Empty)
        {
            return new List<RegistrationDto>();
        }
        try
        {
            var uri = Endpoints.Registration.GetRegistrationsData.Replace("{organisationId}", organisationId.ToString());
            var result = await client.SendGetRequest(uri);
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<RegistrationDto>();
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            var registrations = await result.Content.ReadFromJsonAsync<IEnumerable<RegistrationDto>>(options);
            return registrations ?? new List<RegistrationDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get registration data for organisationId: {OrganisationId}", organisationId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RegistrationOverviewDto>> GetRegistrationsOverviewByOrgIdAsync(Guid? organisationId)
    {
        if (organisationId == Guid.Empty)
        {
            return new List<RegistrationOverviewDto>();
        }
        try
        {
            var uri = Endpoints.Registration.GetRegistrationsOverviewByOrgId.Replace("{organisationId}", organisationId.ToString());
            var result = await client.SendGetRequest(uri);
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<RegistrationOverviewDto>();
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            
            var registrations = await result.Content.ReadFromJsonAsync<IEnumerable<RegistrationOverviewDto>>(options);

            return registrations ?? new List<RegistrationOverviewDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get registration overview data for organisationId: {OrganisationId}", organisationId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateRegistrationSiteAddressAsync(Guid registrationId, UpdateRegistrationSiteAddressDto request)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationSiteAddress.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, request);
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update registration site address and contact details - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateRegistrationTaskStatusAsync(Guid registrationId, UpdateRegistrationTaskStatusDto request)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, request);
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update registration task status - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskItem>> GetRegistrationTaskStatusAsync(Guid? registrationId)
    {
        try
        {
            var uri = Endpoints.Registration.RegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendGetRequest(uri);
            
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            if( result.StatusCode is HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException("Invalid request to get registration task status");
            }

            if (result.StatusCode is HttpStatusCode.NoContent)
            {
                return new List<TaskItem>();
            }

            var response = await result.Content.ReadFromJsonAsync<ApplicantRegistrationTasksDto>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                });

            var tasks = new List<TaskItem>();
            foreach (var task in response!.Tasks)
            {
                tasks.Add(new TaskItem().Create(task.Id, task.TaskName, task.Status));
            }

            return tasks;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get registration task status - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetCountries()
    {
        try
        {
            var uri = Endpoints.Lookup.GetCountries;

            var result = await client.SendGetRequest(uri);

            result.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<IEnumerable<string>>(options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to Get Countries");
            throw;
        }
    }
}