using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IOtherPermitsService
    {
        Task<OtherPermitsDto> GetByRegistrationId(int registrationId);
        Task<OtherPermitsDto> Save(OtherPermitsDto dto);
    }
}
