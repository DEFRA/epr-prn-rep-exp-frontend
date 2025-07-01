using Azure.Core;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Microsoft.Extensions.Options;

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
    public async Task CreateExemptionReferences(CreateExemptionReferencesDto dto)
    {
        try
        {
            var uri = Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences;
            await client.SendPostRequest(uri, dto);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create material exemption references");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<RegistrationMaterialDto>> GetAllRegistrationMaterialsAsync(Guid registrationId)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.GetAllRegistrationMaterials, registrationId);
            var response = await client.SendGetRequest(uri);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await response.Content.ReadFromJsonAsync<List<RegistrationMaterialDto>>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to retrieve registration materials for registration {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RegistrationMaterialDto?> CreateAsync(CreateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(Endpoints.RegistrationMaterial.CreateRegistrationMaterial, request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var created = await result.Content.ReadFromJsonAsync<CreateRegistrationMaterialResponseDto>(options);

            if (created is null)
            {
                return null;
            }

            return new RegistrationMaterialDto
            {
                Id = created.Id,
                RegistrationId = request.RegistrationId,
                IsMaterialBeingAppliedFor = null,
                MaterialLookup = new MaterialLookupDto
                {
                    Name = MaterialItemExtensions.GetMaterialName(request.Material)
                }
            };

        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration {Material} for registration with ID {RegistrationId}", request.Material, request.RegistrationId);
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

    /// <inheritdoc />
    public async Task DeleteAsync(Guid registrationMaterialId)
    {
        try
        {
            await client.SendDeleteRequest(string.Format(Endpoints.RegistrationMaterial.Delete, registrationMaterialId));
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to delete registration material {RegistrationMaterialId}", registrationMaterialId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RegistrationMaterialContactDto> UpsertRegistrationMaterialContactAsync(Guid registrationMaterialId, RegistrationMaterialContactDto request)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationMaterialContact, registrationMaterialId);
            var response = await client.SendPostRequest(uri, request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await response.Content.ReadFromJsonAsync<RegistrationMaterialContactDto>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to upsert registration material for registration material with External ID {Id}", registrationMaterialId);
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
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<List<MaterialsPermitTypeDto>>(options);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get material permit types");
            throw;
        }
    }

	public async Task UpdateIsMaterialRegisteredAsync(List<RegistrationMaterialDto> registrationMaterial)
	{
		try
		{
			List<UpdateIsMaterialRegisteredDto> updateIsMaterialRegisteredDto = registrationMaterial
				.Select(x => new UpdateIsMaterialRegisteredDto { RegistrationMaterialId = x.Id, IsMaterialRegistered = x.IsMaterialBeingAppliedFor })
				.ToList();

			var uri = Endpoints.RegistrationMaterial.UpdateIsMaterialRegistered;
			await client.SendPostRequest(uri, updateIsMaterialRegisteredDto);
		}
		catch (HttpRequestException ex)
		{
			logger.LogError(ex, "Failed to update registration material");
			throw;
		}
	}

    public async Task UpsertRegistrationReprocessingDetailsAsync(Guid registrationMaterialId, RegistrationReprocessingIODto request)
    {
        try
        {
            //var uri = string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationReprocessingDetails, registrationMaterialId);
            //await client.SendPostRequest(uri, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to upsert registration reprocessing details for registration material with External ID {Id}", registrationMaterialId);
            throw;
        }
    }
}