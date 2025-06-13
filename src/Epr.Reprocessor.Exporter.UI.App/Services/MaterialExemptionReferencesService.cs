
namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class MaterialExemptionReferencesService(
    IEprFacadeServiceApiClient facadeClient,
    ILogger<MaterialExemptionReferencesService> logger) : IMaterialExemptionReferencesService
{
    private readonly IEprFacadeServiceApiClient _facadeClient = facadeClient;
    private readonly ILogger<MaterialExemptionReferencesService> _logger = logger;

    public async Task<bool> CreateMaterialExemptionReferences(int registrationMaterialId, List<MaterialExemptionReferenceDto> exemptions)
    {
        try
        {
            var uri = Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences;
            var materialExemptionReferenceDtos = new CreateMaterialExemptionReferenceDto
            {
                RegistrationMaterialId = registrationMaterialId,
                MaterialExemptionReferences = exemptions
            };

            var result = await _facadeClient.SendPostRequest(uri, materialExemptionReferenceDtos);
            var httpResponse = result.EnsureSuccessStatusCode();
            return httpResponse.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create material exemption references");
            return false;
        }
    }
}
