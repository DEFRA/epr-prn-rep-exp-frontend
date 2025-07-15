using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
    public class CheckYourAnswersForNoticeAddressService(IEprFacadeServiceApiClient apiClient,
            ILogger<CheckYourAnswersForNoticeAddressService> logger)
        : BaseExporterService<CheckYourAnswersForNoticeAddressService>(apiClient, logger), ICheckYourAnswersForNoticeAddressService
    {
        public async Task<AddressDto> GetByRegistrationId(Guid registrationId)
        {
            var uri = string.Format(Endpoints.ExporterJourney.LegalAddressGet, Endpoints.CurrentVersion.Version, registrationId);

            var result = await base.Get<AddressDto>(uri);
            return  result;
        }

        public async Task Save(Guid registrationId, AddressDto dto)
        {
            var uri = string.Format(Endpoints.ExporterJourney.LegalAddressPut, Endpoints.CurrentVersion.Version, registrationId);
            await base.Put<AddressDto>(uri, dto);
        }
    }
}
