
namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface IEprFacadeServiceApiClient
    {
        Task<HttpResponseMessage> SendGetRequest(string endpoint);

        Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T body);
        
        Task<HttpResponseMessage> SendDeleteRequest(string endpoint);

        void AddHttpClientHeader(string key, string value);

        void RemoveHttpClientHeader(string key);
    }
}
