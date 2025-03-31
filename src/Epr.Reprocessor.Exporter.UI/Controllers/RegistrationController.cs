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
        private readonly ILogger<RegistrationController> _logger;
        public RegistrationController(ILogger<RegistrationController> logger)
        {
           _logger = logger;
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation()
        {
            //var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            //Todo: set back link to /address-for-legal-documents
            //SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            return View(new UKSiteLocationViewModel());
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
            return RedirectToAction("");
        }

        //[HttpPost]
        //[Route(PagePaths.CountryOfReprocessingSite)]
        //public async Task<ActionResult> UKSiteLocationSaveAndContinue(UKSiteLocationViewModel model)
        //{
            
        //    //SetBackLink(session, PagePaths.CountryOfReprocessingSite);
        //    //save into database here
        //    //Todo: redirect to /application-saved
        //    return RedirectToAction("");
        //}

        private void SetBackLink(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            ViewBag.BackLinkToDisplay = session.Journey.PreviousOrDefault(currentPagePath) ?? string.Empty;
        }

    }
}
