using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class ExporterJourneyWebApiGatewayClient : IExporterJourneyWebApiGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string[] _scopes;
        private readonly ILogger<ExporterJourneyWebApiGatewayClient> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly List<HttpStatusCode> PassThroughExceptions;

        public ExporterJourneyWebApiGatewayClient(
            HttpClient httpClient,
            ITokenAcquisition tokenAcquisition,
            IOptions<HttpClientOptions> httpClientOptions,
            IOptions<WebApiOptions> webApiOptions,
            ILogger<ExporterJourneyWebApiGatewayClient> logger)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _logger = logger;
            _scopes = [webApiOptions.Value.DownstreamScope];
            _httpClient.BaseAddress = new Uri(webApiOptions.Value.BaseEndpoint);
            _httpClient.AddHeaderUserAgent(httpClientOptions.Value.UserAgent);
            _httpClient.AddHeaderAcceptJson();
            PassThroughExceptions ??= new List<HttpStatusCode> { HttpStatusCode.PreconditionRequired };
        }

        public async Task<TOut> Get<TOut>(Guid id, string uri) where TOut : class
        {
            await PrepareAuthenticatedClientAsync();

            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TOut>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting data for id: {0}", id);
                throw;
            }
        }

        public async Task<List<TOut>> Get<TOut>(string uri) where TOut : class
        {
            await PrepareAuthenticatedClientAsync();

            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<TOut>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting data for type: {0}", typeof(TOut).Name);
                throw;
            }
        }

        public async Task Post<TIn>(string uri, TIn payLoad) where TIn : class
        {
            await PrepareAuthenticatedClientAsync();
            var response = await _httpClient.PostAsJsonAsync(uri, payLoad);
            response.EnsureSuccessStatusCode();
        }

        private async Task PrepareAuthenticatedClientAsync()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
               Microsoft.Identity.Web.Constants.Bearer, accessToken);
        }
    }
}
