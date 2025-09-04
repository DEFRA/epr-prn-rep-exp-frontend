using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.Services
{
	[ExcludeFromCodeCoverage]
	public class SaveAndContinueService(IEprFacadeServiceApiClient client, ILogger<SaveAndContinueService> logger) : ISaveAndContinueService
	{

		public async Task AddAsync(SaveAndContinueRequestDto request)
		{
			try
			{
				var result = await client.SendPostRequest(EprPrnFacadePaths.SaveAndContinue, request);

				result.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to add save and continue {Request}", request);
				throw;
			}
		}

		public async Task<SaveAndContinueResponseDto> GetLatestAsync(int registrationId, string controller, string area)
		{
			try
			{
				var result = await client.SendGetRequest($"{EprPrnFacadePaths.SaveAndContinue}/GatLatest/{registrationId}/{area}/{controller}");
				if (result.StatusCode == HttpStatusCode.NotFound)
				{
					return null;
				}
				result.EnsureSuccessStatusCode();

				return await result.Content.ReadFromJsonAsync<SaveAndContinueResponseDto>();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to retrieve get latest for save and continue registrationId :{RegistrationId} area: {Area}", registrationId, area);
			}
			return null;
		}
	}
}