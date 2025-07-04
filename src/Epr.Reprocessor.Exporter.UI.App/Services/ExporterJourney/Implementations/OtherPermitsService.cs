using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
	public class OtherPermitsService(IEprFacadeServiceApiClient apiClient,
			ILogger<OtherPermitsService> logger) : BaseExporterService<OtherPermitsService>(apiClient, logger), IOtherPermitsService
    {
		public async Task<OtherPermitsDto> GetByRegistrationId(Guid registrationId)
        {
            var uri = string.Format(Endpoints.ExporterJourney.OtherPermitsGet, Endpoints.CurrentVersion.Version, registrationId);
            
			var result = await base.Get<OtherPermitsDto>(uri);
			return result;
		}

        public async Task Save(OtherPermitsDto dto)
        {
            var uri = string.Format(Endpoints.ExporterJourney.OtherPermitsPut, Endpoints.CurrentVersion.Version, dto.RegistrationId);
            
            await base.Put<OtherPermitsDto>(uri, dto);
		}
    }
}
