using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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

    public async Task<IEnumerable<UserModel>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId)
    {
        await PrepareAuthenticatedClient();

        var response = await _httpClient.GetAsync($"organisations/all-users?organisationId={organisationId}&serviceRoleId={serviceRoleId}");

        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonWithEnumsAsync<IEnumerable<UserModel>>();

        return roles;
    }

    public async Task<IEnumerable<TeamMembersResponseModel>> GetTeamMembersForOrganisationAsync(string organisationId, int serviceRoleId)
    {
        await PrepareAuthenticatedClient();
        var response = await _httpClient.GetAsync($"organisations/team-members?organisationId={organisationId}&serviceRoleId={serviceRoleId}");

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonWithEnumsAsync<IEnumerable<TeamMembersResponseModel>>();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return GetMockUsersForOrganisationAsync(organisationId);
        }
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

    public static IEnumerable<TeamMembersResponseModel> GetMockUsersForOrganisationAsync(string organisationId)
    {
        string jsonObject = @"[
                  {
                    ""firstName"": ""Rex"",
                    ""lastName"": ""devtenthree"",
                    ""email"": ""ravi.sharma.rexdevthree@eviden.com"",
                    ""personId"": ""0bd42a19-4d81-430f-a59e-7d9688804119"",
                    ""connectionId"": ""bf794f55-f454-4010-a813-32bec7a28f0e"",
                    ""enrolments"": [
                      {
                        ""serviceRoleId"": 11,
                        ""enrolmentStatusId"": 1,
                        ""enrolmentStatusName"": ""Enrolled"",
                        ""serviceRoleKey"": ""Re-Ex.AdminUser"",
                        ""addedBy"": ""Ravi devthree123""
                      },
                      {
                        ""serviceRoleId"": 8,
                        ""enrolmentStatusId"": 1,
                        ""enrolmentStatusName"": ""Enrolled"",
                        ""serviceRoleKey"": ""Re-Ex.ApprovedPerson"",
                        ""addedBy"": ""Rex devtenthree""
                      }
                    ]
                  },
                  {
                    ""firstName"": ""Ravi"",
                    ""lastName"": ""devthree1"",
                    ""email"": ""devthree1@gmail.com"",
                    ""personId"": ""0468480d-cd0a-4114-81ce-6d41ea886c11"",
                    ""connectionId"": ""157128ca-6ede-4068-8c20-1907f9ac9f85"",
                    ""enrolments"": [
                      {
                        ""serviceRoleId"": 8,
                        ""enrolmentStatusId"": 5,
                        ""enrolmentStatusName"": ""Invited"",
                        ""serviceRoleKey"": ""Re-Ex.ApprovedPerson"",
                        ""addedBy"": ""Rex devtenthree""
                      }
                    ]
                  },
                  {
                    ""firstName"": ""Rex"",
                    ""lastName"": ""devtenfive"",
                    ""email"": ""ravi.sharma.rexdevfive@eviden.com"",
                    ""personId"": ""62eb6bd5-df28-4933-ba23-0d430031683e"",
                    ""connectionId"": ""714b8181-367d-4ff2-b9e0-e82725b0cb4d"",
                    ""enrolments"": [
                      {
                        ""serviceRoleId"": 12,
                        ""enrolmentStatusId"": 1,
                        ""enrolmentStatusName"": ""Enrolled"",
                        ""serviceRoleKey"": ""Re-Ex.StandardUser"",
                        ""addedBy"": ""Ravi devthree1""
                      }
                    ]
                  }
                ]
            ";

        return JsonConvert.DeserializeObject<IEnumerable<TeamMembersResponseModel>>(jsonObject);
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