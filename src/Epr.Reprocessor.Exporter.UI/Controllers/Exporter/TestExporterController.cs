namespace Epr.Reprocessor.Exporter.UI.Controllers;
/// <summary>
/// This Controller has been created for test purposes only and should not be used in production. it is to be deleted once the ExporterController is fully implemented.
/// </summary>
/// <param name="SessionManager"></param>
/// <param name="requestMapper"></param>
/// <param name="registrationService"></param>
/// <param name="registrationMaterialService"></param>
[ExcludeFromCodeCoverage(Justification ="Test Controller Only")]
[FeatureGate(FeatureFlags.TestExporter)]
[Route(PagePaths.RegistrationLanding)]
public class TestExporterController(
    ISessionManager<ExporterRegistrationSession> SessionManager,
    IRegistrationService registrationService,
    IRegistrationMaterialService registrationMaterialService,
    IHttpContextAccessor httpContextAccessor) : Controller
{

    [HttpGet]
    [Route("setRegistrationId")]
    public async Task<IActionResult> CreateExporterRegistration2(Guid registrationMaterialId)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = registrationMaterialId;

        await SaveSession(session, "");

        return Ok();
    }

    [HttpGet]
    [Route("create-exporter-registration")]
    public IActionResult CreateExporterRegistration()
    {
        var registrationId = CreateExporterRegistrationIfNotExistsAsync();

        return Ok(registrationId);
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
            var registrationRequest = new CreateRegistrationDto { ApplicationTypeId = 2, OrganisationId = httpContextAccessor.HttpContext.User.GetOrganisationId().Value };
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
            session.ExporterRegistrationApplicationSession.RegistrationMaterialId = created.Id;
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