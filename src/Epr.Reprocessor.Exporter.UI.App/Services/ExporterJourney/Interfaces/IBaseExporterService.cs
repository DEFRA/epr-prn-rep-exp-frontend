namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
	public interface IBaseExporterService
	{
		Task<TOut> Get<TOut>(string uri);
		Task Post<TBody>(string uri, TBody body);
	}
}
