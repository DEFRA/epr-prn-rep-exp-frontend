using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Microsoft.Extensions.Caching.Memory;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class LocalRegistrationService(IMemoryCache memoryCache) : IRegistrationService
{
    public Task<int> CreateAsync(CreateRegistrationDto request)
    {
        memoryCache.CreateEntry("Registration");
        memoryCache.Set("Registration", new RegistrationDto
        {
            Id = 1,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.NewGuid(),
            ReprocessingSiteAddress = new()
            {
                AddressLine1 = "address line 1",
                AddressLine2 = "address line 2",
                TownCity = "Birmingham",
                County = "West Midlands",
                GridReference = "T12345",
                PostCode = "CV1 1TT"
            }
        });

        return Task.FromResult(1);
    }

    public Task<RegistrationDto?> GetAsync(int registrationId)
    {
        return Task.FromResult(memoryCache.Get<RegistrationDto>("Registration"));
    }

    public Task<RegistrationDto?> GetByOrganisationAsync(int applicationTypeId, Guid organisationId)
    {
        return Task.FromResult(memoryCache.Get<RegistrationDto>("Registration"));
    }

    public Task UpdateAsync(int registrationId, UpdateRegistrationRequestDto request)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UpdateRegistrationRequestDto request)
    {
        memoryCache.Set("Registration", request);

        return Task.CompletedTask;
    }

    public Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto request)
    {
        return Task.CompletedTask;
    }

    public Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto request)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId)
    {
        return Task.FromResult<IEnumerable<RegistrationDto>>(new List<RegistrationDto>());
    }

    public Task<List<TaskItem>> GetRegistrationTaskStatusAsync(int registrationId)
    {
        return Task.FromResult<List<TaskItem>>(new List<TaskItem>());
    }
}