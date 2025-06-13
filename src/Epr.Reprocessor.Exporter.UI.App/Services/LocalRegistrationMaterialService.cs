using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Enums;
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

    public Task CreateExemptionReferences(CreateExemptionReferencesDto dto)
    {
        return Task.CompletedTask;
    }

    public Task<int> CreateRegistrationMaterial(int registrationId, string material)
    {
        throw new NotImplementedException();
    }
}