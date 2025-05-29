using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.PostcodeForServiceOfNotices)]
    public async Task<IActionResult> PostcodeForServiceOfNotices()
    {
        var model = GetStubDataFromTempData<PostcodeForServiceOfNoticesViewModel>(SaveAndContinuePostcodeForServiceOfNoticesKey)
                    ?? new PostcodeForServiceOfNoticesViewModel();

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeForServiceOfNotices };

        SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

        await SaveSession(session, PagePaths.PostcodeForServiceOfNotices, PagePaths.RegistrationLanding);

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.PostcodeForServiceOfNotices))
        {
            model = JsonConvert.DeserializeObject<PostcodeForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
        }

        return View(nameof(PostcodeForServiceOfNotices), model);
    }

    [HttpPost]
    [Route(PagePaths.PostcodeForServiceOfNotices)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostcodeForServiceOfNotices(PostcodeForServiceOfNoticesViewModel model, string buttonAction)
    {
        var validationResult = await _validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeForServiceOfNotices };

        SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

        await SaveSession(session, PagePaths.PostcodeForServiceOfNotices, PagePaths.SelectAddressForServiceOfNotices);

        await SaveAndContinue(0, nameof(PostcodeForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinuePostcodeForServiceOfNoticesKey);

        return Redirect(PagePaths.SelectAddressForServiceOfNotices);
    }
}