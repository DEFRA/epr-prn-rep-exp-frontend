using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

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
