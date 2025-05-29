using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public partial class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly ISaveAndContinueService _saveAndContinueService;
        private readonly ISessionManager<ReprocessorExporterRegistrationSession> _sessionManager;
        private readonly IValidationService _validationService;
        private readonly IStringLocalizer<SelectAuthorisationType> _selectAuthorisationStringLocalizer;
        private readonly IRegistrationService _registrationService;
        private readonly IPostcodeLookupService _postcodeLookupService;
        private const string SaveAndContinueAddressForNoticesKey = "SaveAndContinueAddressForNoticesKey";
        private const string SaveAndContinueUkSiteNationKey = "SaveAndContinueUkSiteNationKey";
        private const string SaveAndContinueActionKey = "SaveAndContinue";
        private const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
        private const string SaveAndContinueManualAddressForServiceOfNoticesKey = "SaveAndContinueManualAddressForServiceOfNoticesKey";
        private const string SaveAndContinueSelectAddressForServiceOfNoticesKey = "SaveAndContinueSelectAddressForServiceOfNoticesKey";
        private const string SaveAndContinueManualAddressForReprocessingSiteKey = "SaveAndContinueManualAddressForReprocessingSiteKey";
        private const string SaveAndContinuePostcodeForServiceOfNoticesKey = "SaveAndContinuePostcodeForServiceOfNoticesKey";
        private const string SaveAndContinueAddressOfReprocessingSiteKey = "SaveAndContinueAddressOfReprocessingSiteKey";

        public RegistrationController(
            ILogger<RegistrationController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
            IRegistrationService registrationService,
            IPostcodeLookupService postcodeLookupService,
            IValidationService validationService,
            IStringLocalizer<SelectAuthorisationType> selectAuthorisationStringLocalizer)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            _validationService = validationService;
            _selectAuthorisationStringLocalizer = selectAuthorisationStringLocalizer;
            _registrationService = registrationService;
            _postcodeLookupService = postcodeLookupService;
        }

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
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

        private async Task SetTempBackLink(string previousPagePath, string currentPagePath)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { previousPagePath, currentPagePath };
            SetBackLink(session, currentPagePath);

            await SaveSession(session, previousPagePath, PagePaths.GridReferenceForEnteredReprocessingSite);
        }

        private static List<AddressViewModel> GetListOfAddresses(string postcode)
        {
            var addresses = new List<AddressViewModel>();
            for (int i = 1; i < 11; i++)
            {
                addresses.Add(new AddressViewModel
                {
                    AddressLine1 = $"{i} Test Road",
                    TownOrCity = "Test City",
                    County = "Test County",
                    Postcode = postcode
                });
            }

            return addresses;
        }

        private List<AuthorisationTypes> GetAuthorisationTypes(string nationCode = null)
        {
            var model = new List<AuthorisationTypes> { new()
            {
                Id = 1,
                Name = _selectAuthorisationStringLocalizer["environmental_permit"],
                Label = _selectAuthorisationStringLocalizer["enter_permit_or_license_number"],
                NationCodeCategory = new List<string>(){ NationCodes.England, NationCodes.Wales }
            } , new()
             {
                Id = 2,
                Name = _selectAuthorisationStringLocalizer["installation_permit"],
                Label = _selectAuthorisationStringLocalizer["enter_permit_number"],
                NationCodeCategory = new List<string>(){ NationCodes.England, NationCodes.Wales }
            }, new()
              {
                Id = 3,
                Name = _selectAuthorisationStringLocalizer["pollution_prevention_and_control_permit"],
                Label = _selectAuthorisationStringLocalizer["enter_permit_number"],
                NationCodeCategory = new List<string>(){ NationCodes.Scotland, NationCodes.NorthernIreland }
            }, new()
               {
                Id = 4,
                Name = _selectAuthorisationStringLocalizer["waste_management_licence"],
                Label = _selectAuthorisationStringLocalizer["enter_license_number"],
                NationCodeCategory = new List<string>(){ NationCodes.England, NationCodes.Wales, NationCodes.Scotland, NationCodes.NorthernIreland }
            },
             new()
               {
                Id = 5,
                Name = _selectAuthorisationStringLocalizer["exemption_references"],
                NationCodeCategory = new List<string>(){ NationCodes.England, NationCodes.Wales, NationCodes.Scotland, NationCodes.NorthernIreland }
            }
            };

            model = string.IsNullOrEmpty(nationCode) ? model
                : model.Where(x => x.NationCodeCategory.Contains(nationCode, StringComparer.CurrentCultureIgnoreCase)).ToList();
            return model;
        }


        [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
        private async Task<int> GetRegistrationIdAsync()
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            if (session.RegistrationId.GetValueOrDefault() == 0)
            {
                session.RegistrationId = await CreateRegistrationAsync();
                await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices, PagePaths.RegistrationLanding);
            }

            return session.RegistrationId ?? 0;
        }

        [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
        private async Task<int> CreateRegistrationAsync()
        {
            try
            {
                var dto = new CreateRegistrationDto
                {
                    ApplicationTypeId = 1,
                    OrganisationId = 1,
                };

                return await _registrationService.CreateRegistrationAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to call facade for updateRegistrationSiteAddressDto");
                return 0;
            }
        }
          
        [ExcludeFromCodeCoverage]
        private async Task MarkTaskStatusAsCompleted(RegistrationTaskType taskType)
        {
            var registrationId = await GetRegistrationIdAsync();
            if (registrationId == 0)
            {
                return;
            }

            var updateRegistrationTaskStatusDto = new UpdateRegistrationTaskStatusDto
            {
                TaskName = taskType.ToString(),
                Status = TaskStatuses.Completed.ToString(),
            };

            try
            {
                await _registrationService
                    .UpdateRegistrationTaskStatusAsync(registrationId, updateRegistrationTaskStatusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to call facade for UpdateRegistrationTaskStatusAsync");
            }
        }

        #endregion
    }
}