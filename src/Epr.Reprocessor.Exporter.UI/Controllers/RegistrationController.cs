using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly ISaveAndContinueService _saveAndContinueService;
        private readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
        private const string SaveAndContinueUkSiteNationKey = "SaveAndContinueUkSiteNationKey";
        private const string SaveAndContinueActionKey = "SaveAndContinue";
        private const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

        public RegistrationController(ILogger<RegistrationController> logger, ISaveAndContinueService saveAndContinueService, ISessionManager<ReprocessorExporterRegistrationSession> sessionManager)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation()
        {
            var model = new UKSiteLocationViewModel();
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> {PagePaths.AddressForLegalDocuments,PagePaths.CountryOfReprocessingSite};

            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            await SaveSession(session, PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite);

            //check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            var stubData = TempData.ContainsKey(SaveAndContinueUkSiteNationKey) ? TempData[SaveAndContinueUkSiteNationKey].ToString() : null;

            if (!string.IsNullOrEmpty(stubData)) {
               TempData.Clear();
               model = JsonConvert.DeserializeObject<UKSiteLocationViewModel>(stubData);
            }

            if(saveAndContinue is not null)
            {
                if (saveAndContinue.Action == nameof(RegistrationController.UKSiteLocation) && string.IsNullOrEmpty(stubData))
                {
                    model = JsonConvert.DeserializeObject<UKSiteLocationViewModel>(saveAndContinue.Parameters);
                }
            }

            return View(nameof(UKSiteLocation), model);
        }

        [HttpPost]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<ActionResult> UKSiteLocation(UKSiteLocationViewModel model, string buttonAction)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await SaveAndContinue(0, nameof(UKSiteLocation), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueUkSiteNationKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.PostcodeOfReprocessingSite);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }

        #region private methods
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "error with save and continue {message}", ex.Message);
            }

            //add temp data stubb
            TempData[saveAndContinueTempdataKey] = data;
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
                _logger.LogError(ex, "error with save and continue get latest {message}", ex.Message);
            }
            return null;
        }

        private static void ClearRestOfJourney(ReprocessorExporterRegistrationSession session, string currentPagePath)
        {
            var index = session.Journey.IndexOf(currentPagePath);

            // this also cover if current page not found (index = -1) then it clears all pages
            session.Journey = session.Journey.Take(index + 1).ToList();
        } 

        #endregion
    }
}
