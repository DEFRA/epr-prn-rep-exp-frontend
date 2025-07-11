﻿using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Defines a contract to manage a registration.
/// </summary>
/// <remarks>If you need to manage registration materials then use the <see cref="RegistrationMaterialService"/> as this handles materials.</remarks>
/// <param name="client">The underlying http client that will call the facade.</param>
/// <param name="logger">The logger to log to.</param>
[ExcludeFromCodeCoverage]
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
}