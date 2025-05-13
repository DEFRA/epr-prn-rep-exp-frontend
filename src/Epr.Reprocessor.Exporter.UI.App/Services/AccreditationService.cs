using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

class AccreditationService(IEprFacadeServiceApiClient client, ILogger<AccreditationService> logger) : IAccreditationService
{
    public async Task<Guid> AddAsync(AccreditationRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(EprPrnFacadePaths.Accreditation, request);
            result.EnsureSuccessStatusCode();
            
            var content = await result.Content.ReadAsStringAsync();
            
            return Guid.TryParse(content, out Guid id) ? id : throw new InvalidOperationException($"Failed to parse id from response: {content}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add accreditation {Request}", request);
            throw;
        }
    }
}
