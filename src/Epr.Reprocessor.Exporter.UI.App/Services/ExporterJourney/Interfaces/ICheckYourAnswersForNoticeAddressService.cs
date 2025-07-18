using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces
{
    public interface ICheckYourAnswersForNoticeAddressService
    {
        Task<AddressDto> GetByRegistrationId(Guid registrationId);

        Task Save(Guid registrationId, AddressDto dto);
    }
}