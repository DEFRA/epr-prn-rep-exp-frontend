using Azure.Core;
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
    /// <inheritdoc />
    public async Task CreateExemptionReferences(CreateExemptionReferencesDto request)
    {
        try
        {
            var uri = Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences;
            await client.SendPostRequest(uri, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create material exemption references");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Material> CreateAsync(Guid registrationId, CreateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.CreateRegistrationMaterial, registrationId), request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await result.Content.ReadFromJsonAsync<Material>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration {Material} for registration with ID {RegistrationId}", request.Material, registrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Material> UpdateAsync(Guid registrationId, UpdateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterial, registrationId, request.Material.Id), request);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await result.Content.ReadFromJsonAsync<Material>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update registration material {Material} for registration with ID {RegistrationId}", request.Material.Name, registrationId);
            throw;
        }
    }

    public async Task<int> CreateRegistrationMaterial(int registrationId, string material)
    {
        try
        {
            var uri = "api/v1/RegistrationMaterial/CreateRegistrationMaterial";
            var result = await client.SendPostRequest(uri, new { RegistrationId = registrationId, Material = material });

            return await result.Content.ReadFromJsonAsync<int>();

        }
        catch(HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration material");
            throw;
        }
    }
}