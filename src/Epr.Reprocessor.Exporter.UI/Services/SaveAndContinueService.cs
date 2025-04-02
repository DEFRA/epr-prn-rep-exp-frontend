using Epr.Reprocessor.Exporter.UI.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.Services
{
    public class SaveAndContinueService : ISaveAndContinueService
    {
        public Task SaveAndContinueAsync(int registrationId, string action, string controller, string area, string data)
        {
            // add call to facade endpoint to save user journey in the database
            throw new NotImplementedException();
        }
    }
}
