using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Defines a contract to manage a registration.
/// </summary>
/// <remarks>If you need to manage registration materials then use the <see cref="RegistrationMaterialService"/> as this handles materials.</remarks>
/// <param name="client">The underlying http client that will call the facade.</param>
/// <param name="logger">The logger to log to.</param>
public class ExporterRegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<ExporterRegistrationService> logger) : IExporterRegistrationService
{
    public async Task SaveOverseasReprocessorAsync(OverseasAddressRequestDto request)
    {
        try
        {
            await client.SendPostRequest(Endpoints.RegistrationMaterial.SaveOverseasReprocessor, request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to save the overseas reprocessor");
            throw;
        }
    }

    public async Task<List<OverseasMaterialReprocessingSiteDto>?> GetOverseasMaterialReprocessingSites(Guid RegistrationMaterialId)
    {
        try
        {
            var result = await client.SendGetRequest(string.Format(Endpoints.RegistrationMaterial.GetOverseasMaterialReprocessingSites, Endpoints.CurrentVersion.Version, RegistrationMaterialId));
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<List<OverseasMaterialReprocessingSiteDto>>(options);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to get overseas material reprocessing Sites by registration material id {registrationMaterialId}.", RegistrationMaterialId);
            throw;
        }
    }

    public async Task UpsertInterimSitesAsync(SaveInterimSitesRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(String.Format(Endpoints.RegistrationMaterial.SaveInterimSites, Endpoints.CurrentVersion.Version), request);
            result.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to save the interim sites for registration material id {registrationMaterialId}.", request.RegistrationMaterialId);
            throw;
        }
    }
}