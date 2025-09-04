using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs;

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
    [ExcludeFromCodeCoverage(Justification = "This is a temp method")]
    public async Task<List<RegistrationMaterialDto>> GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(Guid registrationId)
    {
        try
        {
            return await CallGetAllRegistrationMaterialsAsync(registrationId);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to retrieve registration materials for registration {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<RegistrationMaterial>> GetAllRegistrationMaterialsAsync(Guid registrationId)
    {
        try
        {
            var materials = await CallGetAllRegistrationMaterialsAsync(registrationId);

            return materials.Select(MapRegistrationMaterial).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to retrieve registration materials for registration {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RegistrationMaterial?> CreateAsync(CreateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(Endpoints.RegistrationMaterial.CreateRegistrationMaterial, request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new MaterialItemConverter() }
            };

            var created = await result.Content.ReadFromJsonAsync<CreateRegistrationMaterialResponseDto>(options);

            if (created is null)
            {
                return null;
            }

            return new RegistrationMaterial
            {
                Id = created.Id,
                Applied = false,
                Name = request.Material
            };

        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration {Material} for registration with ID {RegistrationId}", request.Material, request.RegistrationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RegistrationMaterial> UpdateAsync(Guid registrationId, UpdateRegistrationMaterialDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterial, registrationId, request.RegistrationMaterial.Id), request);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new MaterialItemConverter() }
            };

            return (await result.Content.ReadFromJsonAsync<RegistrationMaterial>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update registration material {Material} for registration with ID {RegistrationId}", request.RegistrationMaterial.Name, registrationId);
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
            logger.LogError(ex, "Failed to update registration material for registration permits with External ID {Id}", id);
            throw;
        }
    }

    public async Task UpdateRegistrationMaterialPermitCapacityAsync(Guid id, UpdateRegistrationMaterialPermitCapacityDto request)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermitCapacity, id);
            await client.SendPostRequest(uri, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update registration material for registration permit capacity with External ID {Id}", id);
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

    private async Task<List<RegistrationMaterialDto>> CallGetAllRegistrationMaterialsAsync(Guid registrationId)
    {
        var uri = string.Format(Endpoints.RegistrationMaterial.GetAllRegistrationMaterials, registrationId);
        var response = await client.SendGetRequest(uri);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new MaterialItemConverter() }
        };

        var materials = (await response.Content.ReadFromJsonAsync<List<RegistrationMaterialDto>>(options))!;
        return materials;
    }

    private RegistrationMaterial MapRegistrationMaterial(RegistrationMaterialDto materialDto)
    {
        var permit = MapPermit(materialDto);
        return new RegistrationMaterial
        {
            Id = materialDto.Id,
            Name = materialDto.MaterialLookup.Name,
            Status = materialDto.StatusLookup.Status,
            PermitType = permit.permitType,
            PermitPeriod = permit.periodId,
            PermitNumber = permit.permitNumber,
            WeightInTonnes = permit.weightInTonnes.GetValueOrDefault(),
            Applied = materialDto.IsMaterialBeingAppliedFor.GetValueOrDefault(),
            Exemptions = materialDto.ExemptionReferences.Select(MapExemption).ToList()
        };
    }

    private Exemption MapExemption(ExemptionReferencesLookupDto input) =>
        new()
        {
            ReferenceNumber = input.ReferenceNumber
        };

    private static (PermitType? permitType, PermitPeriod? periodId, decimal? weightInTonnes, string? permitNumber) MapPermit(RegistrationMaterialDto material)
    {
        if (material.PermitType?.Id is null or 0)
        {
            return (null, null, null, null);
        }

        return (PermitType)material.PermitType.Id switch
        {
            PermitType.PollutionPreventionAndControlPermit => (
                PermitType.PollutionPreventionAndControlPermit,
                MapPermitPeriod(material.PPCPeriodId),
                material.PPCReprocessingCapacityTonne,
                material.PPCPermitNumber),

            PermitType.WasteManagementLicence => (
                PermitType.WasteManagementLicence,
                MapPermitPeriod(material.WasteManagementPeriodId),
                material.WasteManagementReprocessingCapacityTonne,
                material.WasteManagementLicenceNumber),

            PermitType.InstallationPermit => (
                PermitType.InstallationPermit,
                MapPermitPeriod(material.InstallationPeriodId),
                material.InstallationReprocessingTonne,
                material.InstallationPermitNumber),

            PermitType.EnvironmentalPermitOrWasteManagementLicence => (
                PermitType.EnvironmentalPermitOrWasteManagementLicence,
                MapPermitPeriod(material.EnvironmentalPeriodId),
                material.EnvironmentalPermitWasteManagementTonne,
                material.EnvironmentalPermitWasteManagementNumber),

            PermitType.WasteExemption => (PermitType.WasteExemption, null, null, null),
            PermitType.None => (PermitType.None, null, null, null),
            _ => throw new ArgumentOutOfRangeException(nameof(material))
        };
    }

    private static PermitPeriod MapPermitPeriod(int? permitPeriodId)
    {
        if (permitPeriodId is null)
        {
            return PermitPeriod.None;
        }

        return (PermitPeriod)permitPeriodId;
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
            var uri = string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationReprocessingDetails, registrationMaterialId);
            await client.SendPostRequest(uri, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to upsert registration reprocessing details for registration material with External ID {Id}", registrationMaterialId);
            throw;
        }
    }

    public async Task UpdateMaterialNotReprocessingReasonAsync(Guid registrationMaterialId, string materialNotRegisteringReason)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.UpdateMaterialNotReprocessingReason, registrationMaterialId);
            await client.SendPostRequest(uri, materialNotRegisteringReason);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to update the reason for not registering registration material with External ID {Id}", registrationMaterialId);
            throw;
        }
    }

    public async Task UpdateApplicationRegistrationTaskStatusAsync(Guid registrationMaterialId, UpdateRegistrationTaskStatusDto request)
    {
        try
        {
            var uri = Endpoints.RegistrationMaterial.UpdateApplicationRegistrationTaskStatus.Replace("{registrationMaterialId}", registrationMaterialId.ToString());

            var result = await client.SendPostRequest(uri, request);
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update application registration task status - RegistrationMaterialId: {RegistrationMaterialId}", registrationMaterialId);
            throw;
        }
    }
}