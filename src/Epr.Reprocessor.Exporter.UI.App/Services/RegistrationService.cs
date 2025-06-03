using System.Diagnostics.CodeAnalysis;
using System.Net;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class RegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId)
    {
        var registrations = new List<RegistrationDto>()
        {
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.RegistrationStatus.InProgess,
                AccreditationStatus = Enums.AccreditationStatus.NotAccredited,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 1,
                Material = "Steel",
                SiteId = 1,
                SiteAddress = new DTOs.AddressDto() {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.RegistrationStatus.Granted,
                AccreditationStatus = Enums.AccreditationStatus.InProgress,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 2,
                Material = "Glass",
                SiteId = 1,
                SiteAddress = new DTOs.AddressDto() {
                    Id = 1,
                    AddressLine1 = "12",
                    AddressLine2 = "leylands Road",
                    County = "Leeds"
                }
            }
        };
        return Task.FromResult<IEnumerable<RegistrationDto>>(registrations);
    }

    public async Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model)
    {
        try
        {
            var uri = Endpoints.UpdateRegistrationSiteAddress.Replace("{registrationId}", registrationId.ToString());

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
            var uri = Endpoints.UpdateRegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

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
