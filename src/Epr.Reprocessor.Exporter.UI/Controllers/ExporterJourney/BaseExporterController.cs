using AutoMapper;
using static Epr.Reprocessor.Exporter.UI.App.Constants.Endpoints;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
	[ExcludeFromCodeCoverage]
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class BaseExporterController : Controller
    {
        private readonly ISaveAndContinueService _saveAndContinueService;
        private ExporterRegistrationSession _session;

        protected const string SaveAndContinueActionKey = "SaveAndContinue";
		protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
        protected const string ConfirmAndContinueActionKey = "ConfirmAndContinue";
        protected const string SaveAndContinueLaterActionKey = "SaveAndContinueLater";
        protected readonly ISessionManager<ExporterRegistrationSession> _sessionManager;
		protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;

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
            ILogger logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            _configuration = configuration;
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
                _logger.LogError(ex, "Error with save and continue {Message}", ex.Message);
            }

            //add temp data stub
            if (!string.IsNullOrEmpty(saveAndContinueTempdataKey))
            {
                TempData[saveAndContinueTempdataKey] = data;
            }
        }

        protected async Task SaveSession(string currentPagePath, string? nextPagePath = null)
        {
            if (nextPagePath != null)
            {
                Session.Journey.AddIfNotExists(nextPagePath);
            }

            await _sessionManager.SaveSessionAsync(HttpContext.Session, Session);
        }

        protected async Task SetExplicitBackLink(string previousPagePath, string currentPagePath)
        {
            Session.Journey = [previousPagePath, currentPagePath];
            SetBackLink(currentPagePath);

            await SaveSession(previousPagePath);
        }

        protected void SetBackLink(string currentPagePath)
		{
            var basePath = _configuration["BasePath"] ?? "/";
            var previousPage = Session.Journey!.PreviousOrDefault(currentPagePath) ?? string.Empty;

            // Remove trailing slash from basePath (unless it's just "/")
            if (basePath.Length > 1 && basePath.EndsWith('/'))
                basePath = basePath.TrimEnd('/');

            // Remove leading slash from previousPage
            previousPage = previousPage.TrimStart('/');

            // Combine with a single slash if previousPage is not empty
            ViewBag.BackLinkToDisplay = previousPage.Length > 0
                ? $"{basePath}/{previousPage}"
                : basePath;
        }

        protected async Task PersistJourneyAndSession(string currentPageInJourney, string nextPageInJourney, string area, string controller, string action, string data, string saveAndContinueTempDataKey)
		{
			await SaveSession(currentPageInJourney, nextPageInJourney);
			await PersistJourney(0, action, controller, area, data, saveAndContinueTempDataKey);
		}

        [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story. Plus this has been setup for stubbing")]
        protected async Task InitialiseSession()
        {
            ExporterRegistrationSession session;

            if (await _sessionManager.GetSessionAsync(HttpContext.Session) == null)
            {
                session = new ExporterRegistrationSession();
            }
            else
            {
                session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            }

            _session = session;
        }

        [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story. Plus this has been setup for stubbing")]
        protected async Task<Guid> GetRegistrationIdAsync(Guid? registrationId)
		{
            await InitialiseSession();

            if (Session.RegistrationId != null && registrationId != null && (Session.RegistrationId != registrationId.Value)) 
            {
                Session.RegistrationId = registrationId.Value;
            }
            else if (Session.RegistrationId == null && registrationId != null)
            {
                Session.RegistrationId = registrationId.Value;
            }

            await SaveSession(CurrentPageInJourney, NextPageInJourney);

			if (Session.RegistrationId == null)
			{
				return Guid.Empty;
			}

			return Session.RegistrationId.Value;
		}
    }
}