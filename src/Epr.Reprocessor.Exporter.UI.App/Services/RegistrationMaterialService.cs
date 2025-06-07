namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class RegistrationMaterialService(
    IEprFacadeServiceApiClient facadeClient,
    ILogger<RegistrationMaterialService> logger) : IRegistrationMaterialService
{
    private readonly IEprFacadeServiceApiClient _facadeClient = facadeClient;
    private readonly ILogger<RegistrationMaterialService> _logger = logger;

    public async Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto)
    {
        try
        {
            var uri = Endpoints.RegistrationMaterial.CreateRegistrationMaterialAndExemptionReferences;
            await _facadeClient.SendPostRequest(uri, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create material exemption references");            
        }
    }
}
