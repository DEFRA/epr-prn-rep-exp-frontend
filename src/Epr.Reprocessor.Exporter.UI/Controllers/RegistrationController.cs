using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using EPR.Common.Authorization.Sessions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
	[ExcludeFromCodeCoverage]
	[Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class RegistrationController : Controller
    {
		private readonly ILogger<RegistrationController> _logger;
		private readonly ISaveAndContinueService _saveAndContinueService;
		private readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
        private readonly IValidator<ManualAddressForServiceOfNoticesViewModel> _manualAddressValidator;

        private const string SaveAndContinueUkSiteNationKey = "SaveAndContinueUkSiteNationKey";
		private const string SaveAndContinueActionKey = "SaveAndContinue";
		private const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
        private const string SaveAndContinueManualAddressForServiceOfNoticesKey = "SaveAndContinueManualAddressForServiceOfNoticesKey";

        public RegistrationController(ILogger<RegistrationController> logger,
										 ISaveAndContinueService saveAndContinueService,
										 ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
                                         IValidator<ManualAddressForServiceOfNoticesViewModel> manualAddressValidator)
		{
			_logger = logger;
			_saveAndContinueService = saveAndContinueService;
			_sessionManager = sessionManager;
            _manualAddressValidator = manualAddressValidator;
        }

		[HttpGet]
		[Route(PagePaths.CountryOfReprocessingSite)]
		public async Task<IActionResult> UKSiteLocation()
		{
			var model = new UKSiteLocationViewModel();
			var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
			session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite };

			SetBackLink(session, PagePaths.CountryOfReprocessingSite);

			await SaveSession(session, PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite);

			//check save and continue data
			var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);

			GetStubDataFromTempData(ref model);

			if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.UKSiteLocation))
			{
				model = JsonConvert.DeserializeObject<UKSiteLocationViewModel>(saveAndContinue.Parameters);
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

            return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.PostcodeOfReprocessingSite, PagePaths.ApplicationSaved);
        }

		[HttpGet]
		[Route(PagePaths.NoAddressFound)]
		public async Task<IActionResult> NoAddressFound()
		{
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.PostcodeOfReprocessingSite, PagePaths.NoAddressFound };
            SetBackLink(session, PagePaths.NoAddressFound);
            await SaveSession(session, PagePaths.NoAddressFound, PagePaths.PostcodeOfReprocessingSite);

            var postCode = "[TEST POSTCODE REPLACE WITH SESSION]"; // TODO: Get from session

			var model = new NoAddressFoundViewModel { Postcode = postCode };

			return View(model);
		}

		[HttpGet]
		[Route(PagePaths.PostcodeOfReprocessingSite)]
		public async Task<IActionResult> PostcodeOfReprocessingSite()
		{
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeOfReprocessingSite };
            SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);
            await SaveSession(session, PagePaths.PostcodeOfReprocessingSite, PagePaths.RegistrationLanding);
            
			var model = new PostcodeOfReprocessingSiteViewModel();

			return View(model);
		}

		[HttpPost]
		[Route(PagePaths.PostcodeOfReprocessingSite)]
		public async Task<IActionResult> PostcodeOfReprocessingSite(PostcodeOfReprocessingSiteViewModel model)
		{
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeOfReprocessingSite };
            SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);
            await SaveSession(session, PagePaths.PostcodeOfReprocessingSite, PagePaths.RegistrationLanding);

            // TODO: Wire up to backend
            return View(model);
		}

		[HttpGet]
		[Route(PagePaths.TaskList)]
		public async Task<IActionResult> TaskList()
		{
			var model = new TaskListModel();
			model.TaskList = CreateViewModel();
			return View(model);
		}

        [HttpGet]
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference()
        {
            var model = new ProvideSiteGridReferenceViewModel();
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { "/", PagePaths.GridReferenceForEnteredReprocessingSite };
            SetBackLink(session, PagePaths.GridReferenceForEnteredReprocessingSite);

            await SaveSession(session, "/", PagePaths.GridReferenceForEnteredReprocessingSite);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference(ProvideSiteGridReferenceViewModel model, string buttonAction)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            SetBackLink(session, PagePaths.GridReferenceForEnteredReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return ReturnSaveAndContinueRedirect(buttonAction, "/", "/");
        }

        [HttpGet]
        [Route(PagePaths.ManualAddressForServiceOfNotices)]
        public async Task<IActionResult> ManualAddressForServiceOfNotices()
        {
            var model = GetStubDataFromTempData<ManualAddressForServiceOfNoticesViewModel>(SaveAndContinueManualAddressForServiceOfNoticesKey)
                        ?? new ManualAddressForServiceOfNoticesViewModel();

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.CheckYourAnswersForContactDetails, PagePaths.ManualAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            await SaveSession(session, PagePaths.CheckYourAnswersForContactDetails, PagePaths.ManualAddressForServiceOfNotices);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.ManualAddressForServiceOfNotices))
            {
                model = JsonConvert.DeserializeObject<ManualAddressForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
            }

            return View(nameof(ManualAddressForServiceOfNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.ManualAddressForServiceOfNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualAddressForServiceOfNotices(ManualAddressForServiceOfNoticesViewModel model, string buttonAction)
        {
            var validationResult = await _manualAddressValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.Clear();
                validationResult.AddToModelState(ModelState);
                return View(model);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.CheckYourAnswersForContactDetails);
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

		private void GetStubDataFromTempData(ref UKSiteLocationViewModel? model)
		{
			TempData.TryGetValue(SaveAndContinueUkSiteNationKey, out var tempData);
			if (tempData is not null)
			{
				TempData.Clear();
				model = JsonConvert.DeserializeObject<UKSiteLocationViewModel>(tempData.ToString());
			}
		}

        private T GetStubDataFromTempData<T>(string key)
        {
            TempData.TryGetValue(key, out var tempData);
            if (tempData is not null)
            {
                TempData.Clear();
                return JsonConvert.DeserializeObject<T>(tempData.ToString());
            }

            return default(T);
        }

        private List<TaskItem> CreateViewModel()
		{
			var lst = new List<TaskItem>();
			var sessionData = new TaskListModel();

			// TODO: add logic from data model.
			lst = CalculateTaskListStatus(sessionData);

			return lst;
		}

		private List<TaskItem> CalculateTaskListStatus(TaskListModel sessionData)
		{
			var lst = new List<TaskItem>();
			// if new then use default values
			if (true)
			{
				lst.Add(new TaskItem { TaskName = "Site address and contact details", TaskLink = "#", status = TaskListStatus.NotStart });
				lst.Add(new TaskItem { TaskName = "Waste licenses, permits and exemptions", TaskLink = "#", status = TaskListStatus.CannotStartYet });
				lst.Add(new TaskItem { TaskName = "Reprocessing inputs and outputs", TaskLink = "#", status = TaskListStatus.CannotStartYet });
				lst.Add(new TaskItem { TaskName = "Sampling and inspection plan per material", TaskLink = "#", status = TaskListStatus.CannotStartYet });
				return lst;
			}

			return lst;
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
        #endregion

    }
}
