using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> SessionManager,
    IMapper mapper,
    IRegistrationService registrationService) : Controller
{
    [HttpGet]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public async Task<IActionResult> Index()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        // session.Journey = [PagePaths.TaskList, PagePaths.WastePermitExemptions];

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        var activeOverseasAddress = session.ExporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses?.SingleOrDefault(oa => oa.IsActive);

        OverseasReprocessorSiteViewModel model;
        if (activeOverseasAddress != null)
        {
            model = mapper.Map<OverseasReprocessorSiteViewModel>(activeOverseasAddress);
        }
        else
        { 
            model = new OverseasReprocessorSiteViewModel();
        }

        model.Countries = await registrationService.GetCountries();

        SetBackLink(session, PagePaths.OverseasReprocessorDetails);
        await SaveSession(session, PagePaths.OverseasReprocessorDetails);
        return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public async Task<IActionResult> Index(OverseasReprocessorSiteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Countries = await registrationService.GetCountries();
            return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", model);
        }

        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        var overseasReprocessingSites = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites;

        if (overseasReprocessingSites == null)
        {
            overseasReprocessingSites = new OverseasReprocessingSites();
            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites = overseasReprocessingSites;
        }
        if (overseasReprocessingSites.OverseasAddresses == null)
        {
            overseasReprocessingSites.OverseasAddresses = new List<OverseasAddress>();
        }

        var activeOverseasAddress = overseasReprocessingSites.OverseasAddresses.SingleOrDefault(oa => oa.IsActive);

        if (activeOverseasAddress != null)
        {
            mapper.Map(model, activeOverseasAddress); // Updates properties in-place
        }
        else
        {
            var overseasAddress = mapper.Map<OverseasAddress>(model);
            overseasAddress.IsActive = true;
            overseasReprocessingSites.OverseasAddresses.Add(overseasAddress);
        }

        SetBackLink(session, PagePaths.OverseasReprocessorDetails);
        await SaveSession(session, PagePaths.OverseasReprocessorDetails);
        return RedirectToAction("NextStep"); // Update this with actual next step
    }

    /// <summary>
    /// Save the current session.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    /// <returns>The completed task.</returns>
    protected async Task SaveSession(ExporterRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);
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
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
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
}