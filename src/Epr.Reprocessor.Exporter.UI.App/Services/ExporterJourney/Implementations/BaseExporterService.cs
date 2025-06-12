using Azure.Core;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    [ExcludeFromCodeCoverage]
    public class BaseExporterService<TDerivedClass> : IBaseExporterService
	{
		protected readonly IEprFacadeServiceApiClient ApiClient;

		public BaseExporterService(
			IEprFacadeServiceApiClient apiClient,
			ILogger<TDerivedClass> logger)
		{
			ApiClient = apiClient;
		}

		public async Task<TOut> Get<TOut>(string uri)
		{
			var result = await ApiClient.SendGetRequest(uri);
			result.EnsureSuccessStatusCode();

			return await result.Content.ReadFromJsonAsync<TOut>();
		}

		public async Task Post<TBody>(string uri, TBody body)
		{
			var result = await ApiClient.SendPostRequest(uri, body);
			result.EnsureSuccessStatusCode();
		}
	}
}
