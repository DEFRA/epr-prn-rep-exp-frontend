using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.CountryOfReprocessingSite)]
    public async Task<IActionResult> UKSiteLocation()
    {
        var model = new UKSiteLocationViewModel();
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.AddressForNotices, PagePaths.CountryOfReprocessingSite };

        SetBackLink(session, PagePaths.CountryOfReprocessingSite);

        await SaveSession(session, PagePaths.AddressForNotices, PagePaths.CountryOfReprocessingSite);

        return View(nameof(UKSiteLocation), model);
    }

    [HttpPost]
    [Route(PagePaths.CountryOfReprocessingSite)]
    public async Task<ActionResult> UKSiteLocation(UKSiteLocationViewModel model)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.CountryOfReprocessingSite };

        await SetTempBackLink(PagePaths.AddressForNotices, PagePaths.CountryOfReprocessingSite);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        session.RegistrationApplicationSession.ReprocessingSite?.SetNation((UkNation)model.SiteLocationId!);

        await SaveSession(session, PagePaths.AddressOfReprocessingSite, PagePaths.CountryOfReprocessingSite);

        await SaveAndContinue(0, nameof(UKSiteLocation), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueUkSiteNationKey);

        return Redirect(PagePaths.PostcodeOfReprocessingSite);
    }


}