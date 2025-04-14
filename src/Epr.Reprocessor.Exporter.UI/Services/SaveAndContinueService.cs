using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

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
				var content = await result.Content.ReadAsStringAsync();

				result.EnsureSuccessStatusCode();

			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to add save and continue {request}", request);
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
				logger.LogError(ex, "Failed to retrieve get latest for save and continue registrationId :{registrationId} area: {area}", registrationId, area);
			}
			return null;
		}
	}
}