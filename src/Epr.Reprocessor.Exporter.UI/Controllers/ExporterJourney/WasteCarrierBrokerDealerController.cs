using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterWasteCarrierBrokerDealerRegistration)]
    public class WasteCarrierBrokerDealerController : BaseExporterJourneyPageController<WasteCarrierBrokerDealerRefDto, WasteCarrierBrokerDealerRefViewModel>
    {
        public WasteCarrierBrokerDealerController(
            ILogger<WasteCarrierBrokerDealerController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper,
            IConfiguration configuration,
            IWasteCarrierBrokerDealerRefService wasteCarrierBrokerDealerService)
            : base(logger, saveAndContinueService, sessionManager, mapper, configuration, wasteCarrierBrokerDealerService)
        { }

        protected override string NextPageInJourney => PagePaths.OtherPermits;
        protected override string CurrentPageInJourney => PagePaths.ExporterWasteCarrierBrokerDealerRegistration;
        protected override string SaveAndContinueExporterPlaceholderKey => "SaveAndContinueExporterPlaceholderKey";
        protected override string CurrentPageViewLocation => "~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml";

    }
}
