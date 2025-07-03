using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

public interface IWasteCarrierBrokerDealerRefService
{
    Task<WasteCarrierBrokerDealerRefDto> GetByRegistrationId(Guid registrationId);
    Task SaveAsync(WasteCarrierBrokerDealerRefDto dto);
    Task UpdateAsync(WasteCarrierBrokerDealerRefDto dto);
}
