using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.Mapper;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Exporter;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController : Controller
{
    private readonly ILogger<ExporterController> _logger;
    protected IReprocessorService _reprocessorService { get; }
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

    public ExporterController(
            ILogger<ExporterController> logger,
            ISessionManager<ExporterRegistrationApplicationSession> sessionManager,
            IReprocessorService reprocessorService)
    {
        _logger = logger;
        _reprocessorService = reprocessorService;
    }

    protected ISessionManager<ExporterRegistrationSession> SessionManager { get; }

    [HttpGet]
    [Route(PagePaths.ExporterInterimSiteQuestionOne)]
    public async Task<IActionResult> InterimSitesQuestionOne()
    {
        return View(nameof(InterimSitesQuestionOne), new InterimSitesQuestionOneViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddInterimSites(InterimSitesQuestionOneViewModel model, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        session.ExporterRegistrationApplicationSession.InterimSites.HasInterimSites = model.HasInterimSites;
        SetBackLink(session, PagePaths.ExporterTaskList);
        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);

        if (!ModelState.IsValid) return View(model);


        if (buttonAction == SaveAndContinueActionKey)
        {
            if (model.HasInterimSites)
            {
                return Redirect(PagePaths.Placeholder);
                return View("/confirm-site1");//this will need to be updated once the page we are redirecting to exists
            }

            MarkTaskStatusAsCompleted(TaskType.InterimSites);
            return Redirect(PagePaths.Placeholder);
            return View("/tasklist7");//this will need to be updated once the page we are redirecting to exists
        }

        return Redirect(PagePaths.ApplicationSaved);       
    }

    #region private methods
    [ExcludeFromCodeCoverage]
    private async Task MarkTaskStatusAsCompleted(TaskType taskType)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.RegistrationId is not null)
        {
            var registrationId = session.RegistrationId.Value;
            var updateRegistrationTaskStatusDto = new UpdateRegistrationTaskStatusDto
            {
                TaskName = taskType.ToString(),
                Status = nameof(TaskStatuses.Completed),
            };

            try
            {
                await _reprocessorService.Registrations.UpdateRegistrationTaskStatusAsync(registrationId, updateRegistrationTaskStatusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to call facade for UpdateRegistrationTaskStatusAsync");
                throw;
            }
        }
    }
    #endregion

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
