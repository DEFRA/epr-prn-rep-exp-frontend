using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class WasteCarrierBrokerDealerRefService(IEprFacadeServiceApiClient apiClient,
            ILogger<WasteCarrierBrokerDealerRefService> logger) : BaseExporterService<WasteCarrierBrokerDealerRefService>(apiClient, logger), IWasteCarrierBrokerDealerRefService
    {
        public async Task<WasteCarrierBrokerDealerRefDto> GetByRegistrationId(int registrationId)
        {
            var uri = string.Empty;
            var result = await base.Get<WasteCarrierBrokerDealerRefDto>(uri);
            return result;
        }

        public async Task Save(WasteCarrierBrokerDealerRefDto dto)
        {
            var uri = string.Empty;
            await base.Post<WasteCarrierBrokerDealerRefDto>(uri, dto);
        }
    }
}
