using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    public class WasteCarrierBrokerDealerController : BaseExporterController<WasteCarrierBrokerDealerController>
    {
        public WasteCarrierBrokerDealerController(ILogger<WasteCarrierBrokerDealerController> logger,
                                                  ISaveAndContinueService saveAndContinueService,
                                                  ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
                                                  IMapper mapper) : base(logger, saveAndContinueService, sessionManager, mapper) { }
        [HttpGet]
        [Route(PagePaths.ExporterWasteCarrierBrokerDealerRegistration)]
        public async Task<IActionResult> ExporterWasteCarrierBrokerDealerRegistration()
        {
            // TODO: Set the appropriate previous page
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.ExporterWasteCarrierBrokerDealerRegistration);

            return View(nameof(ExporterWasteCarrierBrokerDealerRegistration), new WasteCarrierBrokerDealerRefViewModel());
        }

        [HttpPost]
        [Route(PagePaths.ExporterWasteCarrierBrokerDealerRegistration)]
        public async Task<IActionResult> ExporterWasteCarrierBrokerDealerRegistration(WasteCarrierBrokerDealerRefViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EnvironmentalPermitOrWasteManagementLicence), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.EnvironmentalPermitOrWasteManagementLicence };

            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            await SaveSession(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence, PagePaths.Placeholder);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.Placeholder);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(EnvironmentalPermitOrWasteManagementLicence), new MaterialPermitViewModel());
        }
    }
}
