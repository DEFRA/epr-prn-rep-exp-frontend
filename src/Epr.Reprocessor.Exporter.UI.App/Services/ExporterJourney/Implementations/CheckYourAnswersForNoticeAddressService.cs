using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class CheckYourAnswersForNoticeAddressService : ICheckYourAnswersForNoticeAddressService
    {
        public Task<AddressDto> GetByRegistrationId(Guid registrationId)
        {
            return Task.FromResult(new AddressDto { AddressLine1 = "1 High Street", TownCity = "London", PostCode = "WC1 2XX", Country = "England" });
        }
    }
}
