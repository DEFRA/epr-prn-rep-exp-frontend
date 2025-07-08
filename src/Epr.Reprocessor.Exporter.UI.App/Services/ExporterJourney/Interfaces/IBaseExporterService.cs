namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface IBaseExporterService<TDto>
    {
        Task<TDto> GetByRegistrationId(Guid registrationId);
        Task Save(TDto dto);
    }
}
