using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

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
    [ExcludeFromCodeCoverage(Justification = " This method need to connect to facade once api is developed till that time UI to work with stub data and have no logic")]
    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId)
    {
        var registrations = new List<RegistrationDto>
        {
            new(){
                Id = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.InProgress,
                AccreditationStatus = AccreditationStatus.NotAccredited,
                ApplicationTypeId = ApplicationType.Reprocessor,
                Year = 2025,
                MaterialId = 1,
                Material = "Steel",
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new DTOs.AddressDto {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.Granted,
                AccreditationStatus = AccreditationStatus.Started,
                ApplicationTypeId = ApplicationType.Reprocessor,
                Year = 2025,
                MaterialId = 2,
                Material = "Glass",
                ReprocessingSiteId = 1,
                RegistrationMaterialId =2,
                ReprocessingSiteAddress = new DTOs.AddressDto {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            }
        };
        return Task.FromResult(registrations.AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task UpdateRegistrationSiteAddressAsync(Guid registrationId, UpdateRegistrationSiteAddressDto model)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationSiteAddress.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, model);
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
    public async Task UpdateRegistrationTaskStatusAsync(Guid registrationId, UpdateRegistrationTaskStatusDto model)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, model);
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

    public async Task<List<TaskItem>> GetRegistrationTaskStatusAsync(int registrationId )
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

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<TaskItem>();
            }

            var data = await result.Content.ReadFromJsonAsync<List<TaskItem>>(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                });
            
            return data ?? new List<TaskItem>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get registration task status - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }
}