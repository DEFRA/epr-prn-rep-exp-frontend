using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [Route(PagePaths.RegistrationLanding)]
    public class RegistrationController : Controller
    {
       private readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;

        //public RegistrationController(ISessionManager<ReprocessorExporterRegistrationSession> sessionManager)
        //{
        //    _sessionManager = sessionManager;
        //}

        public RegistrationController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation() {
            //var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            //Todo: set back link
            //SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            return View(new UKSiteLocationViewModel());
        }

        [HttpPost]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation(UKSiteLocationViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //Todo: redirect to /postcode-of-reprocessing-site
            return RedirectToAction("");
        }

        private void SetBackLink(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            ViewBag.BackLinkToDisplay = session.Journey.PreviousOrDefault(currentPagePath) ?? string.Empty;
        }

    }
}
