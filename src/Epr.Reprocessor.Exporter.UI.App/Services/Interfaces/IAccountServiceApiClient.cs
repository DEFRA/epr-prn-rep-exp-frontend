using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IAccountServiceApiClient
{
    Task<HttpResponseMessage> SendGetRequest(string endpoint);

    Task<HttpResponseMessage> SendGetRequest(Guid organisationId, string endpoint);

    Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T body);

    Task<HttpResponseMessage> PutAsJsonAsync<T>(Guid organisationId, string endpoint, T body);

    Task<OrganisationDetails> GetOrganisationDetailsAsync(string organisationId);

    Task<IEnumerable<UserModel>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId);

    void AddHttpClientHeader(string key, string value);

    void RemoveHttpClientHeader(string key);
}