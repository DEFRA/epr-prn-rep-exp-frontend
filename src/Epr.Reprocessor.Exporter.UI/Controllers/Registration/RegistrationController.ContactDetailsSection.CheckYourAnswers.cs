using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.CheckAnswers)]
    public async Task<IActionResult> CheckAnswers()
    {
        var model = GetStubDataFromTempData<CheckAnswersViewModel>(nameof(CheckAnswers))
                    ?? new CheckAnswersViewModel
                    {
                        SiteLocation = UkNation.England,
                        ReprocessingSiteAddress = new AddressViewModel
                        {
                            AddressLine1 = "2 Rhyl Coast Road",
                            AddressLine2 = string.Empty,
                            TownOrCity = "Rhyl",
                            County = "Denbighshire",
                            Postcode = "SE23 6FH"
                        },
                        SiteGridReference = "AB1234567890",
                        ServiceOfNoticesAddress = new AddressViewModel
                        {
                            AddressLine1 = "10 Rhyl Coast Road",
                            AddressLine2 = string.Empty,
                            TownOrCity = "Rhyl",
                            County = "Denbighshire",
                            Postcode = "SE23 6FH"
                        }
                    };

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.CheckAnswers };

        await SaveSession(session, PagePaths.CheckAnswers, PagePaths.RegistrationLanding);

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.CheckAnswers))
        {
            model = JsonConvert.DeserializeObject<CheckAnswersViewModel>(saveAndContinue.Parameters);
        }

        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.CheckAnswers)]
    public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.CheckAnswers };

        SetBackLink(session, PagePaths.CheckAnswers);

        await SaveSession(session, PagePaths.CheckAnswers, PagePaths.RegistrationLanding);

        await SaveAndContinue(0, nameof(CheckAnswers), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), nameof(CheckAnswers));

        // Mark task status as completed
        await MarkTaskStatusAsCompleted(RegistrationTaskType.SiteAddressAndContactDetails);

        return Redirect(PagePaths.RegistrationLanding);
    }
}