namespace Epr.Reprocessor.Exporter.UI.Services.Interfaces
{
    public interface ISaveAndContinueService
    {
        Task AddAsync(int registrationId, string action, string controller, string area, string data);
    }
}
