using Epr.Reprocessor.Exporter.UI.App.Domain;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Implementation for <see cref="IRegistrationMaterialService"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegistrationMaterialService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationMaterialService> logger) : IRegistrationMaterialService
{
    private readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <inheritdoc />
    public async Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto)
    {
        try
        {
            var uri = Endpoints.RegistrationMaterial.CreateRegistrationMaterialAndExemptionReferences;
            await client.SendPostRequest(uri, dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create material exemption references");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Material> CreateAsync(int registrationId, CreateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.CreateRegistrationMaterial, registrationId), request);

            return (await result.Content.ReadFromJsonAsync<Material>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration {Material} for registration with ID {RegistrationId}", request.Material, registrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Material> UpdateAsync(int registrationId, UpdateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterial, registrationId, request.Material.Id), request);

            return (await result.Content.ReadFromJsonAsync<Material>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update registration material {Material} for registration with ID {RegistrationId}", request.Material.Name, registrationId);
            throw;
        }
    }

    public async Task UpdateRegistrationMaterialPermitsAsync(Guid id, UpdateRegistrationMaterialPermitsDto request)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermits, id);
            await client.SendPostRequest(uri, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update registration material for registration with External ID {Id}", id);
            throw;
        }
    }

    public async Task<List<MaterialsPermitTypeDto>> GetMaterialsPermitTypesAsync()
    {
        try
        {
            var result = await client.SendGetRequest(Endpoints.RegistrationMaterial.GetMaterialsPermitTypes);

            return await result.Content.ReadFromJsonAsync<List<MaterialsPermitTypeDto>>(options);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get material permit types");
            throw;
        }
    }
}