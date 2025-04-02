namespace Epr.Reprocessor.Exporter.UI.Services.Interfaces
{
    public interface ISaveAndContinueService
    {
        Task SaveAndContinueAsync(int registrationId, string action, string controller, string area, string data);
    }
}
