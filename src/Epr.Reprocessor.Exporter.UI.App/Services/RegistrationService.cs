using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class RegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
    public async Task<int> CreateRegistrationAsync(CreateRegistrationDto model)
    {
        try
        {
            var uri = Endpoints.Registration.CreateRegistration;

            var result = await client.SendPostRequest(uri, model);
            result.EnsureSuccessStatusCode();

            if (result.StatusCode == HttpStatusCode.NoContent)
                return default;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<int>(options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create registration");
            throw;
        }
    }

    [ExcludeFromCodeCoverage(Justification = " This method need to connect to facade once api is developed till that time UI to work with stub data and have no logic")]
    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId)
    {
        var registrations = new List<RegistrationDto>()
        {
            new(){
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.InProgress,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.NotAccredited,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 1,
                Material = "Steel",
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.Granted,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.Started,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 2,
                Material = "Glass",
                ReprocessingSiteId = 1,
                RegistrationMaterialId =2,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            }
        };
        return Task.FromResult(registrations.AsEnumerable());
    }

    public async Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model)
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

    public async Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto model)
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
}