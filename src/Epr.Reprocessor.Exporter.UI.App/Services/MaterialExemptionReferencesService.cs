
namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class MaterialExemptionReferencesService(
    IEprFacadeServiceApiClient facadeClient,
    ILogger<MaterialService> logger) : IMaterialExemptionReferencesService
{
    private readonly IEprFacadeServiceApiClient _facadeClient = facadeClient;
    private readonly ILogger<MaterialService> _logger = logger;

    public async Task<bool> CreateMaterialExemptionReferences(List<MaterialExemptionReferenceDto> exemptions)
    {
        try
        {
            var uri = Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences;
            var materialExemptionReferenceDtos = new CreateMaterialExemptionReferenceDto
            {
                MaterialExemptionReferences = exemptions
            };

            var result = await _facadeClient.SendPostRequest(uri, materialExemptionReferenceDtos);
            var httpResponse = result.EnsureSuccessStatusCode();
            return httpResponse.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create material exemption references");
            throw;
        }
    }
}
