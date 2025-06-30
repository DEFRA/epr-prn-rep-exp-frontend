using Epr.Reprocessor.Exporter.UI.App.Services;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[ExcludeFromCodeCoverage]
[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(ISessionManager<ExporterRegistrationSession> sessionManager, IValidationService validationService) : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

    /// <summary>
    /// Save the current session.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    /// <returns>The completed task.</returns>
    protected async Task SaveSession(ExporterRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await sessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Clears the journey in the session from the current page onwards, effectively resetting the journey for the user.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="currentPagePath"></param>
    protected static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
    {
        var index = session.Journey.IndexOf(currentPagePath);

        // this also cover if current page not found (index = -1) then it clears all pages
        session.Journey = session.Journey.Take(index + 1).ToList();
    }

    /// <summary>
    /// Temporary method to set the back link.
    /// </summary>
    /// <remarks>Differs to the SetBackLink method by explicitly setting the previous page rather than using PreviousOrDefault.</remarks>
    /// <param name="previousPagePath">The path to the previous page.</param>
    /// <param name="currentPagePath">The path to the current page.</param>
    /// <returns>The completed page.</returns>
    protected async Task SetTempBackLink(string previousPagePath, string currentPagePath)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        session.Journey = [previousPagePath, currentPagePath];
        SetBackLink(session, currentPagePath);

        await SaveSession(session, previousPagePath);
    }

    /// <summary>
    /// Sets the back link for the current page in the session's journey.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    protected void SetBackLink(ExporterRegistrationSession session, string currentPagePath)
    {
        ViewBag.BackLinkToDisplay = session.Journey!.PreviousOrDefault(currentPagePath) ?? string.Empty;
    }

    /// <summary>
    /// Handles the save and continue handlers.
    /// </summary>
    /// <param name="buttonAction">The name of the handler i.e. SaveAndContinue or SaveAndComeBackLater.</param>
    /// <param name="saveAndContinueRedirectUrl">The url to redirect to for the save and continue handler.</param>
    /// <param name="saveAndComeBackLaterRedirectUrl">The url to redirect to for the save and come back later handler.</param>
    /// <returns>A redirect result.</returns>
    protected RedirectResult ReturnSaveAndContinueRedirect(string buttonAction, string saveAndContinueRedirectUrl, string saveAndComeBackLaterRedirectUrl)
    {
        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(saveAndContinueRedirectUrl);
        }

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(saveAndComeBackLaterRedirectUrl);
        }

        return Redirect("/Error");
    }


    [HttpGet]
    [Route(PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSite()
    {
        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.AddAnotherOverseasReprocessingSite);

        return View(nameof(AddAnotherOverseasReprocessingSite));
    }


    [HttpPost]
    [Route(PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSite(AddAnotherOverseasReprocessingSiteViewModel model, string buttonAction)
    {
        var validationResult = await validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.AddAnotherOverseasReprocessingSite);

        var session =  await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();       

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);

        switch(buttonAction){

            case SaveAndContinueActionKey:

                return Redirect(PagePaths.OverseasSiteDetails);
                break;

            case SaveAndComeBackLaterActionKey:

                return Redirect(PagePaths.CheckYourAnswersOverseasReprocessor);

            default:

                return View(nameof(AddAnotherOverseasReprocessingSite));
                break;
        }        
    }
}