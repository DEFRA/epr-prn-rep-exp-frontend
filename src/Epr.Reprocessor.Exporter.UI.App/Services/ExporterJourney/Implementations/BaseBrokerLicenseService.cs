using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class BaseBrokerLicenseService
    {
        protected readonly IExporterJourneyWebApiGatewayClient ApiClient;

        public BaseBrokerLicenseService(IExporterJourneyWebApiGatewayClient apiClient)
        {
            ApiClient = apiClient;
        }
    }
}
