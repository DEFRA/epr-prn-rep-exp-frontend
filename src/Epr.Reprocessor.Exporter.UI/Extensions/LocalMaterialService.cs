using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

/// <summary>
/// Local development implementation of the material service, used solely for local development to aid in local testing.
/// </summary>
[ExcludeFromCodeCoverage]
public class LocalMaterialService : IMaterialService
{
    public Task<List<MaterialDto>> GetAllMaterialsAsync()
    {
        var materials = new List<MaterialDto>
        {
            new() { Code = "PL", Name = MaterialItem.Plastic },
            new() { Code = "WD", Name = MaterialItem.Wood },
            new() { Code = "AL", Name = MaterialItem.Aluminium }
        };

        return Task.FromResult(materials.OrderBy(o => o.Name.ToString()).ToList());
    }
}