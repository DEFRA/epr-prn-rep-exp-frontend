using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
	[ExcludeFromCodeCoverage]
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class BaseExporterController<TController> : Controller
    {
        private readonly ILogger<TController> _logger;
        private readonly ISaveAndContinueService _saveAndContinueService;

		protected const string SaveAndContinueActionKey = "SaveAndContinue";
		protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
		protected readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
		protected readonly IMapper Mapper;

		protected string PreviousPageInJourney { get; set; }
		protected string NextPageInJourney { get; set; }
		protected string CurrentPageInJourney { get; set; }
		protected ReprocessorExporterRegistrationSession Session
        {
            get
            {
                if (_sessionManager.GetSessionAsync(HttpContext.Session) == null)
                    return new ReprocessorExporterRegistrationSession();
                else
                    return _sessionManager.GetSessionAsync(HttpContext.Session).Result;
            }
        }
            
        public BaseExporterController(
            ILogger<TController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
            IMapper mapper)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            Mapper = mapper;
		}

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
        }

        [HttpGet($"{PagePaths.RegistrationLanding}{PagePaths.ApplicationSaved}", Name = RegistrationRouteIds.ApplicationSaved)]
        protected IActionResult ApplicationSaved() => View();

        protected async Task SetSessionAndBackLink()
        {
            Session.Journey = new List<string> { PreviousPageInJourney, CurrentPageInJourney };
            SetBackLink(CurrentPageInJourney);

            await SaveSession(PreviousPageInJourney, PagePaths.GridReferenceForEnteredReprocessingSite);
        }

        protected async Task SaveAndContinue(int registrationId, string action, string controller, string area, string data, string saveAndContinueTempdataKey)
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



		private async Task<SaveAndContinueResponseDto> GetSaveAndContinue(int registrationId, string controller, string area)
        {
            try
            {
                return await _saveAndContinueService.GetLatestAsync(registrationId, controller, area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with save and continue get latest {Message}", ex.Message);
            }
            return null;
        }

        private static void ClearRestOfJourney(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            var index = session.Journey.IndexOf(currentPagePath);

            // this also cover if current page not found (index = -1) then it clears all pages
            session.Journey = session.Journey.Take(index + 1).ToList();
        }

        private RedirectResult ReturnSaveAndContinueRedirect(string buttonAction, string saveAndContinueRedirectUrl, string saveAndComeBackLaterRedirectUrl)
        {
            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(saveAndContinueRedirectUrl);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(saveAndComeBackLaterRedirectUrl);
            }

            return Redirect("/Error");
        }
    }
}