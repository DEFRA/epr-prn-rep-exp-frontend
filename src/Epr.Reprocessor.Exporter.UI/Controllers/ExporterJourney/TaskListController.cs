using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
[Route(PagePaths.ExporterRegistrationTaskList)]
public class TaskListController(ILogger<TaskListController> logger,
	ISaveAndContinueService saveAndContinueService,
	ISessionManager<ExporterRegistrationSession> sessionManager,
	IMapper mapper) : BaseExporterController<TaskListController>(logger, saveAndContinueService, sessionManager, mapper)
{
	private const string TaskListPageViewLocation = "~/Views/ExporterJourney/TaskList/TaskList.cshtml";
    private const string previousPageInJourney = "/start-exporter-registration";

    [HttpGet]
	public async Task<IActionResult> TaskList()
	{
		//TODO: This might need to update with Exporter journey registartion tasklists (Currently used static list in the UI. ).
		var model = new TaskListModel();

        //model.TaskList.Add(new TaskItem { TaskName = TaskType.SiteAndContactDetails, Status = TaskStatus.NotStart });
        //var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        //session.Journey = [PagePaths.ExporterRegistrationTaskList];

        SetExplicitBackLink(previousPageInJourney, PagePaths.ExporterRegistrationTaskList);

        //await SaveSession(session, PagePaths.TaskList);

        return View(TaskListPageViewLocation, model);
	}
}
