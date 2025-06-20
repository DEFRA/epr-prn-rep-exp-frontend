using System.Reflection;
using Epr.Reprocessor.Exporter.UI.App.Attributes;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Implementation for <see cref="IMaterialService"/>.
/// </summary>
/// <param name="facadeClient">Wrapper client to communicate with the backend.</param>
/// <param name="logger">A logger instance to log to.</param>
public class MaterialService(
    IEprFacadeServiceApiClient facadeClient, 
    ILogger<MaterialService> logger) : IMaterialService
{
    private readonly IEprFacadeServiceApiClient _facadeClient = facadeClient;
    private readonly ILogger<MaterialService> _logger = logger;

    /// <inheritdoc />.
    public async Task<List<MaterialLookupDto>> GetAllMaterialsAsync()
    {
        try
        {
            var response = await _facadeClient.SendGetRequest(Endpoints.Material.GetAllMaterials);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode is HttpStatusCode.NoContent)
            {
                return [];
            }

            var materials = await response.Content.ReadFromJsonAsync<List<MaterialLookupDto>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            if (materials is null)
            {
                _logger.LogWarning("No materials found or deserialization failed.");
                return [];
            }

            materials = materials
                .Where(m => m.Name.GetType()
                    .GetField(m.Name.ToString())?
                    .GetCustomAttribute<MaterialLookupAttribute>()?.IsVisible ?? true).ToList();

            materials = materials.OrderBy(o => o.DisplayText).ToList();

            return materials;

        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(0,  ex, "Failed to call {0}", Endpoints.Material.GetAllMaterials);
            throw;
        }
    }
}