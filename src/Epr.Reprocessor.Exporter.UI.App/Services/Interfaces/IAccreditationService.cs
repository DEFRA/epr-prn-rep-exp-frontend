using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IAccreditationService
{
    Task<Guid> AddAsync(AccreditationRequestDto request);
    Task<AccreditationResponseDto> GetAsync(Guid id);
}
