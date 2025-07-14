using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class CheckYourAnswersForNoticeAddressService(IEprFacadeServiceApiClient apiClient,
            ILogger<CheckYourAnswersForNoticeAddressService> logger)
        : BaseExporterService<CheckYourAnswersForNoticeAddressService>(apiClient, logger), ICheckYourAnswersForNoticeAddressService
    {
        public Task<AddressDto> GetByRegistrationId(Guid registrationId)
        {
            return Task.FromResult(new AddressDto { AddressLine1 = "1 High Street", TownCity = "London", PostCode = "WC1 2XX", Country = "England" });
        }

        public async Task Save(Guid registrationId, AddressDto dto)
        {
            var uri = string.Format(Endpoints.ExporterJourney.LegalAddressPut, Endpoints.CurrentVersion.Version, registrationId);
            await base.Put<AddressDto>(uri, dto);
        }
    }
}
