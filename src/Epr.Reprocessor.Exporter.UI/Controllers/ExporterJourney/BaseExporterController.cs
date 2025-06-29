using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
	[ExcludeFromCodeCoverage]
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class BaseExporterController<TController> : Controller
    {
        private readonly ISaveAndContinueService _saveAndContinueService;
        private ExporterRegistrationSession _session;

        protected const string SaveAndContinueActionKey = "SaveAndContinue";
		protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
		protected readonly ISessionManager<ExporterRegistrationSession> _sessionManager;
		protected readonly IMapper Mapper;
		protected readonly ILogger<TController> Logger;

		protected string PreviousPageInJourney { get; set; }
		protected string NextPageInJourney { get; set; }
		protected string CurrentPageInJourney { get; set; }
		protected ExporterRegistrationSession Session
        {
            get
            {
                if (_session != null)
                    return _session;

                if (HttpContext == null 
                    || HttpContext.Session == null 
                    || _sessionManager.GetSessionAsync(HttpContext.Session) == null)
                    return new ExporterRegistrationSession();
                else
                    return _sessionManager.GetSessionAsync(HttpContext.Session).Result;
            }
        }
            
        public BaseExporterController(
            ILogger<TController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper)
        {
            Logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            Mapper = mapper;
		}

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
        }

        protected IActionResult ApplicationSaved(){
            return View("~/Views/Shared/ApplicationSaved.cshtml");
        }

        protected async Task PersistJourney(int registrationId, string action, string controller, string area, string data, string saveAndContinueTempdataKey)
        {
            try
            {
                await _saveAndContinueService.AddAsync(new App.DTOs.SaveAndContinueRequestDto { Action = action, Area = area, Controller = controller, Parameters = data, RegistrationId = registrationId });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error with save and continue {Message}", ex.Message);
            }

            //add temp data stub
            if (!string.IsNullOrEmpty(saveAndContinueTempdataKey))
            {
                TempData[saveAndContinueTempdataKey] = data;
            }
        }

        protected async Task SaveSession(string currentPagePath, string? nextPagePath)
        {
            ClearRestOfJourney(Session, currentPagePath);

			Session.Journey.AddIfNotExists(nextPagePath);

            await _sessionManager.SaveSessionAsync(HttpContext.Session, Session);
        }

		protected void SetBackLink(string currentPagePath)
		{
            // Backlink behaviour is to go back to the previous page in the journey 
        }

        protected async Task PersistJourneyAndSession(string currentPageInJourney, string nextPageInJourney, string area, string controller, string action, string data, string saveAndContinueTempDataKey)
		{
			await SaveSession(currentPageInJourney, nextPageInJourney);
			await PersistJourney(0, action, controller, area, data, saveAndContinueTempDataKey);
		}

        private static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
        {
            var index = session.Journey.IndexOf(currentPagePath);

            // this also cover if current page not found (index = -1) then it clears all pages
            session.Journey = session.Journey.Take(index + 1).ToList();
        }

		[ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story. Plus this has been setup for stubbing")]
		protected async Task<Guid> GetRegistrationIdAsync(Guid? registrationId)
		{
			var session = await _sessionManager.GetSessionAsync(HttpContext.Session)
				?? new ExporterRegistrationSession { RegistrationId = registrationId };

            if (session.RegistrationId != null && registrationId != null && (session.RegistrationId != registrationId.Value)) 
            { 
                session.RegistrationId = registrationId.Value;
            }
			
            await SaveSession(CurrentPageInJourney, NextPageInJourney);

			if (session.RegistrationId == null)
			{
				return Guid.Empty;
			}
			return session.RegistrationId.Value;
		}
    }
}