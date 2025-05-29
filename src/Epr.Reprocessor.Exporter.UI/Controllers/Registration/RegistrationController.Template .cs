using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.GridReferenceOfReprocessingSite)]
    public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite()
    {
        var model = new ProvideGridReferenceOfReprocessingSiteViewModel();
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();

        session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.GridReferenceOfReprocessingSite);

        await SaveSession(session, PagePaths.GridReferenceOfReprocessingSite, PagePaths.RegistrationLanding);

        var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        if (lookupAddress.SelectedAddressIndex.HasValue)
        {
            await SetTempBackLink(PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);
        }
        else
        {
            await SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);
        }

        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.GridReferenceOfReprocessingSite)]
    public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite(ProvideGridReferenceOfReprocessingSiteViewModel model, string buttonAction)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.GridReferenceOfReprocessingSite };

        await SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

        if (!ModelState.IsValid)
        {
            return View(nameof(ProvideGridReferenceOfReprocessingSite), model);
        }

        session.RegistrationApplicationSession.ReprocessingSite!.SetGridReference(model.GridReference);

        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(PagePaths.AddressForNotices);
        }

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(nameof(ProvideGridReferenceOfReprocessingSite), model);
    }

}