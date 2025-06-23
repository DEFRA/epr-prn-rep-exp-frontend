using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
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
            StubSessionObject();
		}

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
        }

        [HttpGet($"{PagePaths.RegistrationLanding}{PagePaths.ApplicationSaved}", Name = RegistrationRouteIds.ApplicationSaved)]
        protected IActionResult ApplicationSaved() => View();

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
			ViewBag.BackLinkToDisplay = Session.Journey.PreviousOrDefault(currentPagePath) ?? string.Empty;
		}

		protected async Task PersistJourneyAndSession(string currentPageInJourney, string nextPageInJourney, string area, string controller, string action, string data, string saveAndContinueTempDataKey)
		{
			SetBackLink(currentPageInJourney);
			await SaveSession(currentPageInJourney, nextPageInJourney);
			await PersistJourney(0, action, controller, area, data, saveAndContinueTempDataKey);
		}

        private static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
        {
            var index = session.Journey.IndexOf(currentPagePath);

            // this also cover if current page not found (index = -1) then it clears all pages
            session.Journey = session.Journey.Take(index + 1).ToList();
        }

        [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
        protected async Task<Guid> GetRegistrationIdAsync()
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
            if (session.RegistrationId == null)
            {
                // Can we guarantee that the registration will have been created by this point?

                //session.RegistrationId = await CreateRegistrationAsync();
                //await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices);
            }

            if(session.RegistrationId == Guid.Empty)
            {
                return Guid.Empty;
            }
            return session.RegistrationId.Value;
        }

        private void StubSessionObject()
        {
            var session = new ExporterRegistrationSession();
            session.RegistrationId = Guid.Parse("2bd6a43f-9068-4615-86b1-a0fc35603f39");
            
            _session  = session;
        }
    }
}