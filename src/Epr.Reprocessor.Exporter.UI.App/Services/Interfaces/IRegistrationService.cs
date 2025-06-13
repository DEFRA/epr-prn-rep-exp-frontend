using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IRegistrationService
{
    Task<int> CreateRegistrationAsync(CreateRegistrationDto model);
    Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model);
    Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto model);
    Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId);
    Task<List<TaskItem>> GetRegistrationTaskStatusAsync(int registrationId);
}