using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterWasteCarrierBrokerDealerRegistration)]
    public class WasteCarrierBrokerDealerController(
            ILogger<OtherPermitsController> logger,
			ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
			IMapper mapper,
            IWasteCarrierBrokerDealerRefService service) : BaseExporterController<OtherPermitsController>(logger, saveAndContinueService, sessionManager, mapper)
    {
        private const string PreviousPageInJourney = PagePaths.ExporterPlaceholder;
        private const string NextPageInJourney = PagePaths.ExporterPlaceholder;
        private const string CurrentPageInJourney = PagePaths.OtherPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";
        private readonly IWasteCarrierBrokerDealerRefService _service = service;

        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml";

        // TODO: remove the following
        private Guid HardCodedRegistrationIdFromDev12 = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

        [HttpGet]
        public async Task<IActionResult> Get(Guid? registrationId)
        {
            registrationId = await GetRegistrationIdAsync(registrationId);
            // TODO: remove the following
            registrationId = HardCodedRegistrationIdFromDev12;

            SetBackLink(PagePaths.ExporterWasteCarrierBrokerDealerRegistration);

            var dto = await _service.GetByRegistrationId(registrationId.Value);
            var vm = dto == null ? new WasteCarrierBrokerDealerRefViewModel { RegistrationId = registrationId.Value } : Mapper.Map<WasteCarrierBrokerDealerRefViewModel>(dto);

            return View(CurrentPageViewLocation, vm);

        }

        [HttpPost]
        public async Task<IActionResult> Post(WasteCarrierBrokerDealerRefViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(CurrentPageViewLocation, viewModel);
            }

            try
            {
                var dto = Mapper.Map<WasteCarrierBrokerDealerRefDto>(viewModel);
                _service.Save(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to save Waste Carrier, Broker or Dealer Reference");
                throw;
            }

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(ExporterPlaceholder),
                nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            SetBackLink(PagePaths.ExporterWasteCarrierBrokerDealerRegistration);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return Redirect(PagePaths.OtherPermits);

                case SaveAndComeBackLaterActionKey:
                    return Redirect(PagePaths.ApplicationSaved);

                default:
                    return Redirect(PagePaths.ExporterPlaceholder);
            }
        }
    }
}
