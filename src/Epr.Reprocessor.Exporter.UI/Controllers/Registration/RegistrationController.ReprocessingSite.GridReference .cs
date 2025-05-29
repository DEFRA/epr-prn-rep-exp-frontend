using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{

    [HttpGet]
    [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
    public async Task<IActionResult> ProvideSiteGridReference()
    {
        var model = new ProvideSiteGridReferenceViewModel();
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.GridReferenceForEnteredReprocessingSite };

        session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.GridReferenceForEnteredReprocessingSite);
        await SaveSession(session, PagePaths.GridReferenceForEnteredReprocessingSite, PagePaths.RegistrationLanding);

        SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
    public async Task<IActionResult> ProvideSiteGridReference(ProvideSiteGridReferenceViewModel model, string buttonAction)
    {
        SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return ReturnSaveAndContinueRedirect(buttonAction, "/", PagePaths.ApplicationSaved);
    }
}