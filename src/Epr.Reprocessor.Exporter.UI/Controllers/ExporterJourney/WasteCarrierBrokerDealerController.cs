using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpGet]
        public async Task<IActionResult> Get(Guid registrationId)
        {
            // TODO: I think the registration id is in session at this point and should not be passed in
            await GetRegistrationIdAsync(registrationId);

            SetBackLink(PagePaths.ExporterWasteCarrierBrokerDealerRegistration);

            var dto = await _service.GetByRegistrationId(registrationId);
            var vm = dto == null ? new WasteCarrierBrokerDealerRefViewModel { RegistrationId = registrationId } : Mapper.Map<WasteCarrierBrokerDealerRefViewModel>(dto);

            return View("~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml", vm);

        }

        [HttpPost]
        public async Task<IActionResult> Post(WasteCarrierBrokerDealerRefViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View("ExporterWasteCarrierBrokerDealerReference", viewModel);
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
