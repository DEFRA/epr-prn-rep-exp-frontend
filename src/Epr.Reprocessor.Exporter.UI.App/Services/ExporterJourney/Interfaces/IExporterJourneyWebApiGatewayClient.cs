namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IExporterJourneyWebApiGatewayClient
    {
        Task<TOut> Get<TOut>(Guid id, string uri) where TOut : class;
        Task<List<TOut>> Get<TOut>(string uri) where TOut : class;
        Task Post<TIn>(string uri, TIn payLoad) where TIn : class;
    }
}
