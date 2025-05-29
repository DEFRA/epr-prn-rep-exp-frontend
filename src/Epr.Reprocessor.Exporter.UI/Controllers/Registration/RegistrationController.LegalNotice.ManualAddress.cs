using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.ManualAddressForServiceOfNotices)]
    public async Task<IActionResult> ManualAddressForServiceOfNotices()
    {
        var model = GetStubDataFromTempData<ManualAddressForServiceOfNoticesViewModel>(SaveAndContinueManualAddressForServiceOfNoticesKey)
                    ?? new ManualAddressForServiceOfNoticesViewModel();

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.ManualAddressForServiceOfNotices };

        SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

        await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices, PagePaths.RegistrationLanding);

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.ManualAddressForServiceOfNotices))
        {
            model = JsonConvert.DeserializeObject<ManualAddressForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
        }

        return View(nameof(ManualAddressForServiceOfNotices), model);
    }

    [HttpPost]
    [Route(PagePaths.ManualAddressForServiceOfNotices)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManualAddressForServiceOfNotices(ManualAddressForServiceOfNoticesViewModel model, string buttonAction)
    {
        var validationResult = await _validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.ManualAddressForServiceOfNotices };

        SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

        await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices, PagePaths.RegistrationLanding);

        await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueManualAddressForServiceOfNoticesKey);

        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(PagePaths.RegistrationLanding);
        }
        else if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }
}