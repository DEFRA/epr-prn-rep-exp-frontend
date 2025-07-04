using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IOtherPermitsService: IBaseExporterService<OtherPermitsDto>
    {
        Task<OtherPermitsDto> GetByRegistrationId(Guid registrationId);
        Task Save(OtherPermitsDto dto);
    }
}
