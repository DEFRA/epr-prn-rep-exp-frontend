using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IOtherPermitsService
    {
        Task<OtherPermitsDto> GetByRegistrationId(Guid registrationId);
        Task Save(OtherPermitsDto dto);
    }
}
