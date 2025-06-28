using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;

public class WasteCarrierBrokerDealerRefService(IEprFacadeServiceApiClient apiClient,
        ILogger<WasteCarrierBrokerDealerRefService> logger) : BaseExporterService<WasteCarrierBrokerDealerRefService>(apiClient, logger), IWasteCarrierBrokerDealerRefService
{
    public async Task<WasteCarrierBrokerDealerRefDto> GetByRegistrationId(Guid registrationId)
    {
        var uri = string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefGet, Endpoints.CurrentVersion.Version, registrationId);

        var result = await base.Get<WasteCarrierBrokerDealerRefDto>(uri);
        return result;
    }

    public async Task Save(WasteCarrierBrokerDealerRefDto dto)
    {
        var uri = string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, dto.RegistrationId);

        await base.Post<WasteCarrierBrokerDealerRefDto>(uri, dto);
    }

    public async Task Update(WasteCarrierBrokerDealerRefDto dto)
    {
        var uri = string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPut, Endpoints.CurrentVersion.Version, dto.RegistrationId);

        await base.Put<WasteCarrierBrokerDealerRefDto>(uri, dto);
    }
}
