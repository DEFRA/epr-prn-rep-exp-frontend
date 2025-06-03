using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class BrokerLicenseService : BaseBrokerLicenseService, IBrokerLicenseService
    {

        public BrokerLicenseService(IExporterJourneyWebApiGatewayClient apiClient) : base(apiClient)
        {             
        }

        public async Task<BrokerLicenseDto> GetDetails(Guid id)
        {
            var uri = string.Empty;
            var result = await ApiClient.Get<BrokerLicenseDto>(id, uri);

            return result;
        }
    }
}
