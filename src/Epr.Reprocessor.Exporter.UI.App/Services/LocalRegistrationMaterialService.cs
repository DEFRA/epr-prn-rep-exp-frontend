using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Microsoft.Extensions.Caching.Memory;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class LocalRegistrationMaterialService(IMemoryCache memoryCache) : IRegistrationMaterialService
{
    public Task<Material> CreateAsync(int registrationId, CreateRegistrationMaterialDto request)
    {
        var material = new Material
        {
            Id = 1,
            Name = MaterialItem.Steel,
            Applied = false
        };

        memoryCache.CreateEntry("RegistrationMaterial");
        memoryCache.Set("RegistrationMaterial", material);

        return Task.FromResult(material);
    }

    public Task<Material> UpdateAsync(int registrationId, UpdateRegistrationMaterialDto request)
    {
        memoryCache.Set("RegistrationMaterial", request);

        return Task.FromResult(new Material
        {
            Id = 1,
            Name = MaterialItem.Steel,
            Applied = false
        });
    }

    public Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto)
    {
        return Task.CompletedTask;
    }

    public Task UpdateRegistrationMaterialPermitsAsync(Guid externalId, UpdateRegistrationMaterialPermitsDto request)
    {
        memoryCache.Set("RegistrationMaterialPermits", request);

        return Task.CompletedTask;
    }

    public Task<List<MaterialsPermitTypeDto>> GetMaterialsPermitTypesAsync()
    {
        var items = new List<MaterialsPermitTypeDto>
        {
            new()
            {
                Id = (int)MaterialPermitType.EnvironmentalPermitOrWasteManagementLicence,
                Name = "Environmental permit or waste management licence",
            },

            new()
            {
                Id = (int)MaterialPermitType.InstallationPermit,
                Name = "Installation Permit",
            },

            new()
            {
                Id = (int)MaterialPermitType.PollutionPreventionAndControlPermit,
                Name = "Pollution,Prevention and Control(PPC) permit",
            },

            new()
            {
                Id = (int)MaterialPermitType.WasteManagementLicence,
                Name = "Waste Management Licence",
            },

            new()
            {
                Id = (int)MaterialPermitType.WasteExemption,
                Name = "Waste Exemption",
            }
         };

        return Task.FromResult(items);
    }
}