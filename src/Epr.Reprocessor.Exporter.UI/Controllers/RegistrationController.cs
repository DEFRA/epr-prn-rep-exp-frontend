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
        private readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
        public RegistrationController(ILogger<RegistrationController> logger, IUserJourneySaveAndContinueService userJourneySaveAndContinueService, ISessionManager<ReprocessorExporterRegistrationSession> sessionManager)
        {
            _logger = logger;
            _userJourneySaveAndContinueService = userJourneySaveAndContinueService;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation()
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> {PagePaths.AddressForLegalDocuments,PagePaths.CountryOfReprocessingSite};

            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

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

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);

            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            return Redirect(PagePaths.PostcodeOfReprocessingSite);
        }

        [HttpPost]
        public async Task<ActionResult> UKSiteLocationSaveAndContinue(UKSiteLocationViewModel model)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);

            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            await SaveAndContinue(nameof(UKSiteLocation), nameof(RegistrationController), JsonConvert.SerializeObject(model));

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
