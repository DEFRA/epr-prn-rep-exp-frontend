using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IRegistrationService
{
    Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model);
    Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto model);
}