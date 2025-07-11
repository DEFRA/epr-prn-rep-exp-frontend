using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
[Route(PagePaths.ExporterRegistrationTaskList)]
public class TaskListController(ILogger<TaskListController> logger,
	ISaveAndContinueService saveAndContinueService,
	ISessionManager<ExporterRegistrationSession> sessionManager,
	IMapper mapper,
    IRegistrationService registrationService) : BaseExporterController<TaskListController>(logger, saveAndContinueService, sessionManager, mapper)
{
	private const string TaskListPageViewLocation = "~/Views/ExporterJourney/TaskList/TaskList.cshtml";
    private const string previousPageInJourney = "/start-exporter-registration";

    private readonly IRegistrationService _registrationService = registrationService;

    [HttpGet]
	public async Task<IActionResult> TaskList()
	{
		//TODO: This might need to update with Exporter journey registartion tasklists (Currently used static list in the UI. ).
		var model = new TaskListModel();

        Guid? registrationId = null;
        WasteCarrierBrokerDealerRefDto? dto = null;

        await InitialiseSession();

        //model.TaskList.Add(new TaskItem { TaskName = TaskType.SiteAndContactDetails, Status = TaskStatus.NotStart });
        //var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        //session.Journey = [PagePaths.ExporterRegistrationTaskList];

        var userData = User.GetUserData();
        var organisation = userData.Organisations.FirstOrDefault();
        var organisationId = organisation != null ? organisation.Id.Value : Guid.Empty;

        var createRegistration = new CreateRegistrationDto
        {
            ApplicationTypeId = 1,
            OrganisationId = organisationId,
        };

        if (organisation != null)
        {
            createRegistration.ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = organisation.BuildingNumber,
                AddressLine2 = organisation.Street,
                TownCity = organisation.Town,
                County = organisation.County,
                Country = organisation.Country,
                PostCode = organisation.Postcode,
                NationId = organisation.NationId,
                GridReference = string.Empty
            };
        }

        var response = await _registrationService.CreateAsync(createRegistration);
        registrationId = response.Id;

        await GetRegistrationIdAsync(registrationId.Value);

        SetExplicitBackLink(previousPageInJourney, PagePaths.ExporterRegistrationTaskList);

        await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(ExporterPlaceholderController),
            nameof(Index), null, null);

        //await SaveSession(session, PagePaths.TaskList);

        return View(TaskListPageViewLocation, model);
	}
}
