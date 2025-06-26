using Epr.Reprocessor.Exporter.UI.Mapper;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Exporter;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController : Controller
{
    private readonly ILogger<ExporterController> _logger;

    public ExporterController(
            ILogger<ExporterController> logger,
            ISessionManager<ExporterRegistrationApplicationSession> sessionManager,
            IReprocessorService reprocessorService)
    {
        _logger = logger;
    }

    protected ISessionManager<ExporterRegistrationSession> SessionManager { get; }

    [HttpGet]
    [Route(PagePaths.ExporterInterimSiteQuestionOne)]
    public async Task<IActionResult> InterimSiteQuestionOne()
    {
        //var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();

        //SetBackLink(session, PagePaths.ExporterTaskList);

        return View(nameof(InterimSiteQuestionOne), new InterimSitesModel());
    }


    [HttpPost]
    public async Task<IActionResult> AddInterimSites(InterimSitesModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        session.ExporterRegistrationApplicationSession.InterimSites.UseInterimSites = model.UseInterimSites;
        SetBackLink(session, PagePaths.ExporterTaskList);
        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);
        return View(nameof(MaximumWeightSiteCanReprocess), new MaximumWeightSiteCanReprocessViewModel());
    }

    protected void SetBackLink(ExporterRegistrationSession session, string currentPagePath)
    {
        ViewBag.BackLinkToDisplay = session.Journey!.PreviousOrDefault(currentPagePath) ?? string.Empty;
    }

    protected async Task SaveSession(ExporterRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    protected static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
    {
        var index = session.Journey.IndexOf(currentPagePath);

        // this also cover if current page not found (index = -1) then it clears all pages
        session.Journey = session.Journey.Take(index + 1).ToList();
    }
}
