using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IAccountServiceApiClient
{
    Task<HttpResponseMessage> SendGetRequest(string endpoint);

    Task<HttpResponseMessage> SendGetRequest(Guid organisationId, string endpoint);

    Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T body);

    Task<HttpResponseMessage> PutAsJsonAsync<T>(Guid organisationId, string endpoint, T body);

    Task<IEnumerable<UserModel>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId);

    IEnumerable<TeamMembersResponseModel> GetMockUsersForOrganisationAsync(string organisationId, int serviceRoleId);

    Task<IEnumerable<TeamMembersResponseModel>> GetTeamMembersForOrganisationAsync(string organisationId);

    void AddHttpClientHeader(string key, string value);

    void RemoveHttpClientHeader(string key);
}