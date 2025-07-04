using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterCheckYourAnswersPermits)]
    public class CheckYourAnswersPermitsController : BaseExporterJourneyPageController<
        CheckYourAnswersPermitsController, IOtherPermitsService, OtherPermitsDto, OtherPermitsViewModel>
    {
        public CheckYourAnswersPermitsController(
            ILogger<CheckYourAnswersPermitsController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper,
            IConfiguration configuration,
            IOtherPermitsService otherPermitsService)
            : base(logger, saveAndContinueService, sessionManager, mapper, configuration, otherPermitsService)
        { }

        protected override string NextPageInJourney => PagePaths.ExporterPlaceholder;
        protected override string CurrentPageInJourney => PagePaths.ExporterCheckYourAnswersPermits;
        protected override string SaveAndContinueExporterPlaceholderKey => "SaveAndContinueExporterPlaceholderKey";
        protected override string CurrentPageViewLocation => "~/Views/ExporterJourney/CheckYourAnswersPermits/CheckYourAnswersPermits.cshtml";
    }
}

