using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface ISaveAndContinueService
    {
        Task AddAsync(SaveAndContinueRequestDto request);
        Task<SaveAndContinueResponseDto> GetLatestAsync(int registrationId, string area);
    }
}
