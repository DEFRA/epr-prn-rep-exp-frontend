namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IBaseExporterService<TDto>
    {
		// Task<TOut> Get<TOut>(string uri);
		// Task Post<TBody>(string uri, TBody body);
        // Task Put<TBody>(string uri, TBody body);
        Task<TDto> GetByRegistrationId(Guid registrationId);
        Task Save(TDto dto);
    }
}
