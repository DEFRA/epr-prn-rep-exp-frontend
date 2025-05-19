using System.Text.Json;
using Azure.Core;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class AccreditationService(IEprFacadeServiceApiClient client, ILogger<AccreditationService> logger) : IAccreditationService
{
    private readonly JsonSerializerOptions Serializer = new ()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<Guid> AddAsync(AccreditationRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(EprPrnFacadePaths.Accreditation, request);
            result.EnsureSuccessStatusCode();
            
            var content = await result.Content.ReadAsStringAsync();

            var dto = JsonSerializer.Deserialize<AccreditationResponseDto>(content, Serializer);
            return dto.ExternalId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add accreditation {Request}", request);
            throw;
        }
    }

    public async Task<AccreditationResponseDto> GetAsync(Guid id)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.Accreditation}/{id}");
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            var dto = JsonSerializer.Deserialize<AccreditationResponseDto>(content, Serializer);

            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch accreditation details with Id: {Id}", id);
            throw;
        }
    }
}
