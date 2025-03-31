using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [Route(PagePaths.RegistrationLanding)]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IUserJourneySaveAndContinueService _userJourneySaveAndContinueService;
        public RegistrationController(ILogger<RegistrationController> logger, IUserJourneySaveAndContinueService userJourneySaveAndContinueService)
        {
           _logger = logger;
            _userJourneySaveAndContinueService = userJourneySaveAndContinueService;
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation()
        {
            //var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            //Todo: set back link to /address-for-legal-documents
            //SetBackLink(session, PagePaths.CountryOfReprocessingSite);
            var model = new UKSiteLocationViewModel();
            return View(nameof(UKSiteLocation), model);
        }

        [HttpPost]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<ActionResult> UKSiteLocation(UKSiteLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            //Todo: redirect to /postcode-of-reprocessing-site
            return Redirect(PagePaths.PostcodeOfReprocessingSite);
        }

        [HttpPost]
        public async Task<ActionResult> UKSiteLocationSaveAndContinue(UKSiteLocationViewModel model)
        {
            //SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            await SaveAndContinue(nameof(UKSiteLocation), nameof(RegistrationController), JsonConvert.SerializeObject(model));
            //Todo: redirect to /application-saved

            return Redirect(PagePaths.ApplicationSaved);
        }

        private void SetBackLink(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            ViewBag.BackLinkToDisplay = session.Journey.PreviousOrDefault(currentPagePath) ?? string.Empty;
        }

        private async Task SaveAndContinue(string action, string controller, string data)
        {
            await _userJourneySaveAndContinueService.SaveAndContinueAsync(action, controller, data);
        }

    }
}
