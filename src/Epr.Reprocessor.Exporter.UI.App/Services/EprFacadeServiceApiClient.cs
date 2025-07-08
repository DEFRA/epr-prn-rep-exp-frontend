using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.App.Services
{
    [ExcludeFromCodeCoverage]
    public class EprFacadeServiceApiClient : IEprFacadeServiceApiClient
    {
        private const string EprOrganisationHeader = "X-EPR-Organisation";

        private readonly HttpClient _httpClient;
        

        public EprFacadeServiceApiClient(HttpClient httpClient, IOptions<EprPrnFacadeApiOptions> options)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendGetRequest(string endpoint)
        {
            await PrepareAuthenticatedClient();

            if (Uri.TryCreate(endpoint, UriKind.Relative, out var validEndpointUri))
            {
                return await _httpClient.GetAsync(endpoint);
            }
            throw new ArgumentException("Invalid endpoint format, possibly malicious");
        }

        public async Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T body)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.PostAsJsonAsync(endpoint, body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> SendPutRequest<T>(string endpoint, T body)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.PutAsJsonAsync(endpoint, body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> SendDeleteRequest(string endpoint)
        {
            await PrepareAuthenticatedClient();

            if (Uri.TryCreate(endpoint, UriKind.Relative, out var validEndpointUri))
            {
                var response = await _httpClient.DeleteAsync(validEndpointUri);
                response.EnsureSuccessStatusCode();
                return response;
            }

            throw new ArgumentException("Invalid endpoint format, possibly malicious");
        }

        public void AddHttpClientHeader(string key, string value)
        {
            RemoveHttpClientHeader(key);
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public void RemoveHttpClientHeader(string key)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(key))
            {
                _httpClient.DefaultRequestHeaders.Remove(key);
            }
        }

        private async Task PrepareAuthenticatedClient()
        {
            _httpClient.AddHeaderAcceptJson();
        }
    }
}