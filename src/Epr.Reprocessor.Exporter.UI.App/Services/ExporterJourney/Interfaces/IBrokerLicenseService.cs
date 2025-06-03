using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IBrokerLicenseService
    {
        Task<BrokerLicenseDto> GetDetails(Guid id);
    }
}
