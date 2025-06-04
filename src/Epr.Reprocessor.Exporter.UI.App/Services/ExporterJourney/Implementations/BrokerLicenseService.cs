using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class BrokerLicenseService : BaseExporterJourneyService, IBrokerLicenseService
    {

        public BrokerLicenseService() : base()
        {             
        }

        public async Task<BrokerLicenseDto> GetDetails(Guid id)
        {
            var uri = string.Empty;
            var result = new BrokerLicenseDto();

            return result;
        }
    }
}
