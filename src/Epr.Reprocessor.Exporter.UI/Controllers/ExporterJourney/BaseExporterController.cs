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
        protected readonly ILogger<TController> _logger;
        protected readonly ISaveAndContinueService _saveAndContinueService;
        protected readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
        protected readonly IMapper _mapper;

        private const string SaveAndContinueActionKey = "SaveAndContinue";
        private const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

        public BaseExporterController(
            ILogger<TController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
            IMapper mapper)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            _mapper = mapper;
        }

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
        }

        protected async Task SetTempBackLink(string previousPagePath, string currentPagePath)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { previousPagePath, currentPagePath };
            SetBackLink(session, currentPagePath);

            await SaveSession(session, previousPagePath, PagePaths.GridReferenceForEnteredReprocessingSite);
        }

        private void SetBackLink(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            ViewBag.BackLinkToDisplay = session.Journey.PreviousOrDefault(currentPagePath) ?? string.Empty;
        }

        private async Task SaveAndContinue(int registrationId, string action, string controller, string area, string data, string saveAndContinueTempdataKey)
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

        private async Task SaveSession(ReprocessorExporterRegistrationSession session, string currentPagePath, string? nextPagePath)
        {
            ClearRestOfJourney(session, currentPagePath);

            session.Journey.AddIfNotExists(nextPagePath);

            await _sessionManager.SaveSessionAsync(HttpContext.Session, session);
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