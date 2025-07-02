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
        private const string NextPageInJourney = PagePaths.OtherPermits;
        private const string CurrentPageInJourney = PagePaths.ExporterWasteCarrierBrokerDealerRegistration;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";
        private readonly IWasteCarrierBrokerDealerRefService _service = service;

        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml";

        [HttpGet]
        public async Task<IActionResult> Get(Guid? registrationId = null)
        {
			registrationId = await GetRegistrationIdAsync(registrationId);

            await SetExplicitBackLink(PagePaths.ExporterPlaceholder, CurrentPageInJourney);

            WasteCarrierBrokerDealerRefViewModel vm = null;
            try
            {
                var dto = await _service.GetByRegistrationId(registrationId.Value);
                if (dto != null) vm = Mapper.Map<WasteCarrierBrokerDealerRefViewModel>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to retrieve Waste Carrier, Broker or Dealer Reference for registration {RegistrationId}", registrationId.Value);
            }
            finally {
                if (vm == null) vm = new WasteCarrierBrokerDealerRefViewModel { RegistrationId = registrationId.Value };
            }
            
            return View(CurrentPageViewLocation, vm);
        }

        [ExcludeFromCodeCoverage(Justification = "To be completed after QA testing")]
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

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(WasteCarrierBrokerDealerController),
                nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return Redirect(PagePaths.OtherPermits);

                case SaveAndComeBackLaterActionKey:
					return ApplicationSaved();

				default:
                    return Redirect(PagePaths.ExporterPlaceholder);
            }
        }
    }
}
