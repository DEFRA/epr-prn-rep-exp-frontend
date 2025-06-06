using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IRegistrationService
{
    Task<int> CreateAsync(CreateRegistrationDto model);

    Task<RegistrationDto?> GetAsync(int registrationId);

    Task<RegistrationDto?> GetByOrganisationAsync(int applicationTypeId, int organisationId);

    Task<RegistrationDto> UpdateAsync(UpdateRegistrationRequestDto request);

    Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model);

    Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto model);

    Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid organisationId);
}

[ExcludeFromCodeCoverage]
public class UpdateRegistrationRequestDto : RegistrationDto
{
}

public interface IRegistrationMaterialService
{

}

public interface IReprocessorService
{
    public IRegistrationService Registrations { get; }

    public IRegistrationMaterialService RegistrationMaterials { get; }
}

public class ReprocessorService : IReprocessorService
{
    public ReprocessorService(IRegistrationService registrationService, IRegistrationMaterialService registrationMaterials)
    {
        RegistrationMaterials = registrationMaterials;
        Registrations = registrationService;
    }

    public IRegistrationService Registrations { get; }

    public IRegistrationMaterialService RegistrationMaterials { get; }
}