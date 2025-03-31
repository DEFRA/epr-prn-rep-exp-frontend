namespace Epr.Reprocessor.Exporter.UI.Services.Interfaces
{
    public interface IUserJourneySaveAndContinueService
    {
        Task SaveAndContinueAsync(string action, string controller, string data);
    }
}
