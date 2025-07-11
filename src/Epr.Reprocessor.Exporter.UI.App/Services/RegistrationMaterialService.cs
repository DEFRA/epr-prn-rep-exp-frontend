using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

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
    public async Task<List<RegistrationMaterial>> GetAllRegistrationMaterialsAsync(Guid registrationId)
    {
        try
        {
            var uri = string.Format(Endpoints.RegistrationMaterial.GetAllRegistrationMaterials, registrationId);
            var response = await client.SendGetRequest(uri);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new MaterialItemConverter() }
            };

            var materials = (await response.Content.ReadFromJsonAsync<List<RegistrationMaterialDto>>(options))!;

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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task UpdateMaximumWeightCapableForReprocessingAsync(
        Guid registrationMaterialId,
        decimal weightInTonnes,
        PeriodDuration period)
    {
        try
        {
            var url = string.Format(Endpoints.RegistrationMaterial.UpdateMaximumWeight, registrationMaterialId);
            await client.SendPutRequest(url, new UpdateMaximumWeightRequestDto
            {
                WeightInTonnes = weightInTonnes,
                PeriodId = (int)period
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get material permit types");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTaskStatusAsync(Guid registrationMaterialId, TaskType taskName, ApplicantRegistrationTaskStatus status)
    {
        try
        {
            var url = string.Format(Endpoints.RegistrationMaterial.UpdateTaskStatus, registrationMaterialId);

            var request = new UpdateRegistrationTaskStatusDto
            {
                TaskName = taskName.ToString(),
                Status = status.ToString()
            };

            var result = await client.SendPostRequest(url, request);
            result.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not update task.");
            throw;
        }
    }

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
            MaxCapableWeightInTonnes = materialDto.MaximumReprocessingCapacityTonne,
            MaxCapableWeightPeriodDuration = (PeriodDuration?)materialDto.MaximumReprocessingPeriodId,
            Applied = materialDto.IsMaterialBeingAppliedFor.GetValueOrDefault(),
            Exemptions = materialDto.ExemptionReferences.Select(MapExemption).ToList()
        };
    }

    private Exemption MapExemption(ExemptionReferencesLookupDto input) =>
        new()
        {
            ReferenceNumber = input.ReferenceNumber
        };

    private static PermitPeriod MapPermitPeriod(int? permitPeriodId)
    {
        if (permitPeriodId is null)
        {
            return PermitPeriod.None;
        }

        return (PermitPeriod)permitPeriodId;
    }
}