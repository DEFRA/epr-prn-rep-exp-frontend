using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Humanizer;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.OtherPermits)]
    public class OtherPermitsController : BaseExporterJourneyPageController<OtherPermitsDto, OtherPermitsViewModel>
    {
        public OtherPermitsController(
            ILogger<OtherPermitsController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper,
            IConfiguration configuration,
            IOtherPermitsService otherPermitsService)
            : base(logger, saveAndContinueService, sessionManager, mapper, configuration, otherPermitsService)
        { }

        protected override string NextPageInJourney => PagePaths.ExporterCheckYourAnswersPermits;
        protected override string CurrentPageInJourney => PagePaths.OtherPermits;
        protected override string SaveAndContinueExporterPlaceholderKey => "SaveAndContinueExporterPlaceholderKey";
        protected override string CurrentPageViewLocation => "~/Views/ExporterJourney/OtherPermits/OtherPermits.cshtml";

        protected override async Task<OtherPermitsDto> GetDtoAsync(Guid registrationId)
        {
            var dto = await _service.GetByRegistrationId(registrationId);
            UpsizeListToNumberOfItems(dto.WasteExemptionReference, 5);
            return dto;
        }

        private static void UpsizeListToNumberOfItems(List<string> list, int maxCount)
        {
            if (list == null) return;
            while (list.Count < maxCount)
            {
                list.Add(string.Empty);
            }
        }
    }
}
