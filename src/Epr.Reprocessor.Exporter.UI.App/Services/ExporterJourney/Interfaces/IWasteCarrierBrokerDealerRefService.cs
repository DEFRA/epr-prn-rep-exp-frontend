using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IWasteCarrierBrokerDealerRefService: IBaseExporterService<WasteCarrierBrokerDealerRefDto>
    {
        Task<WasteCarrierBrokerDealerRefDto> GetByRegistrationId(Guid registrationId);
        Task Save(WasteCarrierBrokerDealerRefDto dto);
    }
}
