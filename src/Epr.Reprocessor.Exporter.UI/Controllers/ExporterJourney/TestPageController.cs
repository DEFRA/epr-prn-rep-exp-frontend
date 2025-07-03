using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    public class TestPageController : ExporterJourneyPageController<
        OtherPermitsController, IOtherPermitsService, OtherPermitsDto, OtherPermitsViewModel>
    {
        public TestPageController(
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

        protected override Task<OtherPermitsDto> GetDtoAsync(Guid registrationId) =>
            _service.GetByRegistrationId(registrationId);

        protected override Task SaveDtoAsync(OtherPermitsDto dto) =>
            _service.Save(dto);
    }

}
