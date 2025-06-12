using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations
{
	public class OtherPermitsService(IEprFacadeServiceApiClient apiClient,
			ILogger<OtherPermitsService> logger) : BaseExporterService<OtherPermitsService>(apiClient, logger), IOtherPermitsService
    {
		public async Task<OtherPermitsDto> GetByRegistrationId(int registrationId)
        {
			var uri = string.Empty;
			var result = await base.Get<OtherPermitsDto>(uri);
			return result;
		}

        public async Task Save(OtherPermitsDto dto)
        {
			var uri = string.Empty;
			await base.Post<OtherPermitsDto>(uri, dto);
		}
    }
}
