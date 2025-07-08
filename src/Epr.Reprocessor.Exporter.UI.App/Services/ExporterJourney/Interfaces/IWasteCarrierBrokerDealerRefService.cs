using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

public interface IWasteCarrierBrokerDealerRefService : IBaseExporterService<WasteCarrierBrokerDealerRefDto>
{
    Task Update(WasteCarrierBrokerDealerRefDto dto);
}