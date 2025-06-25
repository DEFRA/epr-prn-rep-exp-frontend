using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> SessionManager,
    IRequestMapper requestMapper,
    IRegistrationService registrationService,
    IRegistrationMaterialService registrationMaterialService) : Controller
{
    [HttpGet]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public async Task<IActionResult> Index()
    {
        var overseasReprocessorSiteViewModel = new OverseasReprocessorSiteViewModel();
        overseasReprocessorSiteViewModel.Countries = await registrationService.GetCountries();


        //TODO: Populate the model with session data if available

        return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", overseasReprocessorSiteViewModel);
    }

    [HttpPost]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public IActionResult Index(OverseasReprocessorSiteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", model);
        }

        // TODO: Save to session or service

        return RedirectToAction("NextStep"); // Update this with actual next step
    }

    [HttpGet]
    [Route(PagePaths.CreateExporterRegistration)]
    public IActionResult CreateExporterRegistration()
    {
        var registrationId = CreateExporterRegistrationIfNotExistsAsync();

        return Ok();
    }

    protected async Task<Guid> CreateExporterRegistrationIfNotExistsAsync()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session is null)
        {
            session = new ExporterRegistrationSession();
            await SaveSession(session, PagePaths.MaximumWeightSiteCanReprocess);
        }

        if (session!.RegistrationId is null)
        {
            var registrationRequest = await requestMapper.MapExporterForCreate();
            var registration = await registrationService.CreateAsync(registrationRequest);

            session.CreateRegistration(registration.Id);
        }

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);

        var request = new CreateRegistrationMaterialDto
        {
            RegistrationId = session!.RegistrationId.Value,
            Material = "Plastic"
        };

        var created = await registrationMaterialService.CreateAsync(request);
        if (created is not null)
        {
            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.RegistrationMaterialId = created.Id;
        }

        return session!.RegistrationId.Value;
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

}