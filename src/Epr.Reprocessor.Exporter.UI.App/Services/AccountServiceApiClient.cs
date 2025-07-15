using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class AccountServiceApiClient : IAccountServiceApiClient
{
    private const string EprOrganisationHeader = "X-EPR-Organisation";

    private readonly HttpClient _httpClient;
    private readonly string[] _scopes;
    private readonly string _baseAddress;
    private readonly ITokenAcquisition _tokenAcquisition;

    public AccountServiceApiClient(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IOptions<AccountsFacadeApiOptions> options)
    {
        _httpClient = httpClient;
        _tokenAcquisition = tokenAcquisition;
        _scopes = new[] { options.Value.DownstreamScope };
        _baseAddress = options.Value.BaseEndpoint;
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

    public async Task<HttpResponseMessage> SendGetRequest(Guid organisationId, string endpoint)
    {
        await PrepareAuthenticatedClient();

        HttpRequestMessage request = new(HttpMethod.Get, new Uri(endpoint, UriKind.RelativeOrAbsolute));

        request.Headers.Add(EprOrganisationHeader, organisationId.ToString());

        var result = await _httpClient.SendAsync(request, CancellationToken.None);

        result.EnsureSuccessStatusCode();

        return result;
    }

    public async Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T body)
    {
        await PrepareAuthenticatedClient();

        var response = await _httpClient.PostAsJsonAsync(endpoint, body);
        response.EnsureSuccessStatusCode();

        return response;
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(Guid organisationId, string endpoint, T body)
    {
        await PrepareAuthenticatedClient();

        HttpRequestMessage request = new(HttpMethod.Put, new Uri(endpoint, UriKind.RelativeOrAbsolute))
        {
            Content = JsonContent.Create(body)
        };

        request.Headers.Add(EprOrganisationHeader, organisationId.ToString());

        var result = await _httpClient.SendAsync(request, CancellationToken.None);

        result.EnsureSuccessStatusCode();

        return result;
    }

    public async Task<OrganisationDetails?> GetOrganisationDetailsAsync(Guid organisationId)
    {
        // organisations/organisation-with-persons/
        await PrepareAuthenticatedClient();

        var response = await _httpClient.GetAsync($"organisations/organisation-with-persons/{organisationId}");

        response.EnsureSuccessStatusCode();

        var organisationDetails = await response.Content.ReadFromJsonWithEnumsAsync<OrganisationDetails>();

        return organisationDetails;
    }

    public async Task<IEnumerable<UserModel>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId)
    {
        await PrepareAuthenticatedClient();

        var response = await _httpClient.GetAsync($"organisations/all-users?organisationId={organisationId}&serviceRoleId={serviceRoleId}");

        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonWithEnumsAsync<IEnumerable<UserModel>>();

        return roles;
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
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_baseAddress);
        }
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        _httpClient.AddHeaderAcceptJson();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Microsoft.Identity.Web.Constants.Bearer, accessToken);
    }
}