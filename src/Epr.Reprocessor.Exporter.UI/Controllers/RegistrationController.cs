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
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Epr.Reprocessor.Exporter.UI.Enums;
using Microsoft.Extensions.Localization;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services;

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
        private readonly IValidationService _validationService;
        private readonly IStringLocalizer<SelectAuthorisationType> _selectAuthorisationStringLocalizer;
        private readonly IMapper _mapper;
        private readonly IRegistrationService _registrationService;
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
            IValidationService validationService,
            IStringLocalizer<SelectAuthorisationType> selectAuthorisationStringLocalizer,
            IMapper mapper)
        {
            _logger = logger;
            _saveAndContinueService = saveAndContinueService;
            _sessionManager = sessionManager;
            _validationService = validationService;
            _selectAuthorisationStringLocalizer = selectAuthorisationStringLocalizer;
            _registrationService = registrationService;
            _mapper = mapper;
        }

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
        }

        [HttpGet]
        [Route(PagePaths.Placeholder)]
        public async Task<IActionResult> Placeholder()
        {
            return View(nameof(Placeholder));
        }

        [HttpPost]
        [Route(PagePaths.MaximumWeightSiteCanReprocess)]
        public async Task<IActionResult> MaximumWeightSiteCanReprocess(MaximumWeightSiteCanReprocessViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(MaximumWeightSiteCanReprocess), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.MaximumWeightSiteCanReprocess };

            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.MaximumWeightSiteCanReprocess);

            await SaveSession(session, PagePaths.MaximumWeightSiteCanReprocess, PagePaths.Placeholder);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.Placeholder);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(MaximumWeightSiteCanReprocess), new MaximumWeightSiteCanReprocessViewModel());
        }

        [HttpGet]
        [Route(PagePaths.MaximumWeightSiteCanReprocess)]
        public async Task<IActionResult> MaximumWeightSiteCanReprocess()
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.MaximumWeightSiteCanReprocess);

            return View(nameof(MaximumWeightSiteCanReprocess), new MaximumWeightSiteCanReprocessViewModel());
        }

        [HttpPost]
        [Route(PagePaths.EnvironmentalPermitOrWasteManagementLicence)]
        public async Task<IActionResult> EnvironmentalPermitOrWasteManagementLicence(MaterialPermitViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EnvironmentalPermitOrWasteManagementLicence), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.EnvironmentalPermitOrWasteManagementLicence };

            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            await SaveSession(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence, PagePaths.Placeholder);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.Placeholder);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(EnvironmentalPermitOrWasteManagementLicence), new MaterialPermitViewModel());
        }

        [HttpGet]
        [Route(PagePaths.EnvironmentalPermitOrWasteManagementLicence)]
        public async Task<IActionResult> EnvironmentalPermitOrWasteManagementLicence()
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            return View(nameof(EnvironmentalPermitOrWasteManagementLicence), new MaterialPermitViewModel());
        }

        [HttpPost]
        [Route(PagePaths.InstallationPermit)]
        public async Task<IActionResult> InstallationPermit(MaterialPermitViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(InstallationPermit), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.InstallationPermit };

            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.InstallationPermit);

            await SaveSession(session, PagePaths.InstallationPermit, PagePaths.Placeholder);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.Placeholder);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(InstallationPermit), new MaterialPermitViewModel());
        }

        [HttpGet]
        [Route(PagePaths.InstallationPermit)]
        public async Task<IActionResult> InstallationPermit()
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.InstallationPermit);

            return View(nameof(InstallationPermit), new MaterialPermitViewModel());
        }

        [HttpPost]
        [Route(PagePaths.PpcPermit)]
        public async Task<IActionResult> PpcPermit(MaterialPermitViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(PpcPermit), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.PpcPermit };

            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.PpcPermit);

            await SaveSession(session, PagePaths.PpcPermit, PagePaths.Placeholder);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.Placeholder);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(PpcPermit), new MaterialPermitViewModel());
        }

        [HttpGet]
        [Route(PagePaths.PpcPermit)]
        public async Task<IActionResult> PpcPermit()
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.PpcPermit);

            return View(nameof(PpcPermit), new MaterialPermitViewModel());
        }

        [HttpGet]
        [Route(PagePaths.WastePermitExemptions)]
        public async Task<IActionResult> WastePermitExemptions()
        {
            var model = new WastePermitExemptionsViewModel();

            SetTempBackLink(PagePaths.AddressForLegalDocuments, PagePaths.WastePermitExemptions);
            
            model.Materials.AddRange([
                new SelectListItem { Value = "AluminiumR4", Text = "Aluminium (R4)"  },
                new SelectListItem { Value = "GlassR5", Text = "Glass (R5)"  },
                new SelectListItem { Value = "PaperR3", Text = "Paper, board or fibre-based composite material (R3)" },
                new SelectListItem { Value = "PlasticR3", Text = "Plastic (R3)" },
                new SelectListItem { Value = "SteelR4", Text = "Steel (R4)" },
                new SelectListItem { Value = "WoodR3", Text = "Wood (R3)" }
            ]);

            return View("WastePermitExemptions", model);
        }

        [HttpPost]
        [Route(PagePaths.WastePermitExemptions)]
        public async Task<IActionResult> WastePermitExemptions(WastePermitExemptionsViewModel model, string buttonAction)
        {
            SetTempBackLink(PagePaths.AddressForLegalDocuments, PagePaths.WastePermitExemptions);

            if (model.SelectedMaterials.Count == 0)
            {
                ModelState.AddModelError(nameof(model.SelectedMaterials), "Select all the material categories the site has a permit or exemption to accept and recycle");
            }

            if (!ModelState.IsValid)
            {
                model.Materials.AddRange([
                    new SelectListItem { Value = "AluminiumR4", Text = "Aluminium (R4)"  },
                    new SelectListItem { Value = "GlassR5", Text = "Glass (R5)"  },
                    new SelectListItem { Value = "PaperR3", Text = "Paper, board or fibre-based composite material (R3) R3" },
                    new SelectListItem { Value = "PlasticR3", Text = "Plastic (R3)" },
                    new SelectListItem { Value = "SteelR4", Text = "Steel (R4)" },
                    new SelectListItem { Value = "WoodR3", Text = "Wood (R3)" }
                ]);
                return View("WastePermitExemptions", model);
            }
            // TODO: Wire up backend / perform next step
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(PagePaths.AddressForNotices)]
        public async Task<IActionResult> AddressForNotices()
        {
            var model = new AddressForNoticesViewModel
            {
                AddressToShow = new AddressViewModel
                {
                    AddressLine1 = "23 Ruby Street",
                    AddressLine2 = "",
                    TownOrCity = "London",
                    County = "UK",
                    Postcode = "EE12 345"
                }
            };
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.AddressForNotices };

            SetBackLink(session, PagePaths.AddressForNotices);

            await SaveSession(session, PagePaths.GridReferenceOfReprocessingSite, PagePaths.AddressForNotices);

            //check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);

            GetStubDataFromTempData<AddressForNoticesViewModel>(SaveAndContinueAddressForNoticesKey);

            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.AddressForNotices))
            {
                model = JsonConvert.DeserializeObject<AddressForNoticesViewModel>(saveAndContinue.Parameters);
            }
            return View(nameof(AddressForNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.AddressForNotices)]
        public async Task<IActionResult> AddressForNotices(AddressForNoticesViewModel model, string buttonAction)
        {
            if (model.SelectedAddressOptions == 0)
            {
                ModelState.AddModelError(nameof(model.SelectedAddressOptions), "Select an address for service of notices.");
            }

            if (!ModelState.IsValid)
            {
                var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
                session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.AddressForNotices };

                SetBackLink(session, PagePaths.AddressForNotices);
                model = new AddressForNoticesViewModel
                {
                    AddressToShow = new AddressViewModel
                    {
                        AddressLine1 = "23 Ruby Street",
                        AddressLine2 = "",
                        TownOrCity = "London",
                        County = "UK",
                        Postcode = "EE12 345"
                    }
                };

                return View(nameof(AddressForNotices), model);
            }
            // TODO: Wire up backend / perform next step
            throw new NotImplementedException();

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
            SetTempBackLink(PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite);
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

        [HttpGet]
        [Route(PagePaths.TaskList)]
        public async Task<IActionResult> TaskList()
        {
            var model = new TaskListModel();
            model.TaskList = CreateViewModel();
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
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference()
        {
            var model = new ProvideSiteGridReferenceViewModel();

            SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference(ProvideSiteGridReferenceViewModel model, string buttonAction)
        {
            SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return ReturnSaveAndContinueRedirect(buttonAction, "/", PagePaths.ApplicationSaved);
        }

        [HttpGet]
        [Route(PagePaths.ManualAddressForServiceOfNotices)]
        public async Task<IActionResult> ManualAddressForServiceOfNotices()
        {
            var model = GetStubDataFromTempData<ManualAddressForServiceOfNoticesViewModel>(SaveAndContinueManualAddressForServiceOfNoticesKey)
                        ?? new ManualAddressForServiceOfNoticesViewModel();

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.ManualAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices, PagePaths.RegistrationLanding);

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
            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.ManualAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices, PagePaths.RegistrationLanding);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueManualAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.RegistrationLanding);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }

        [HttpGet]
        [Route(PagePaths.GridReferenceOfReprocessingSite)]
        public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite()
        {
            var model = new ProvideGridReferenceOfReprocessingSiteViewModel();

            SetTempBackLink(PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.GridReferenceOfReprocessingSite)]
        public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite(ProvideGridReferenceOfReprocessingSiteViewModel model, string buttonAction)
        {
            SetTempBackLink(PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return ReturnSaveAndContinueRedirect(buttonAction, "/", PagePaths.ApplicationSaved);
        }

        [HttpGet]
        [Route(PagePaths.SelectAddressForServiceOfNotices)]
        public async Task<IActionResult> SelectAddressForServiceOfNotices()
        {
            var model = GetStubDataFromTempData<SelectAddressForServiceOfNoticesViewModel>(SaveAndContinueSelectAddressForServiceOfNoticesKey)
                        ?? new SelectAddressForServiceOfNoticesViewModel();

            // TEMP 
            if (model.Addresses?.Count == 0)
            {
                model.Postcode = "G5 0US";
                model.SelectedIndex = null;
                model.Addresses = GetListOfAddresses(model.Postcode);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.SelectAddressForServiceOfNotices);

            await SaveSession(session, PagePaths.SelectAddressForServiceOfNotices, PagePaths.RegistrationLanding);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.SelectAddressForServiceOfNotices))
            {
                model = JsonConvert.DeserializeObject<SelectAddressForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
            }

            return View(nameof(SelectAddressForServiceOfNotices), model);
        }

        [HttpGet]
        [Route(PagePaths.SelectedAddressForServiceOfNotices)]
        public async Task<IActionResult> SelectedAddressForServiceOfNotices([FromQuery] SelectedAddressViewModel selectedAddress)
        {
            var model = GetStubDataFromTempData<SelectAddressForServiceOfNoticesViewModel>(SaveAndContinueSelectAddressForServiceOfNoticesKey)
                        ?? new SelectAddressForServiceOfNoticesViewModel();

            model.SelectedIndex = selectedAddress.SelectedIndex;

            // TEMP 
            if (model.Addresses?.Count == 0)
            {
                model.Postcode = string.IsNullOrWhiteSpace(selectedAddress.Postcode) ? "G5 0US" : selectedAddress.Postcode;
                model.Addresses = GetListOfAddresses(model.Postcode);
            }

            var validationResult = await _validationService.ValidateAsync(selectedAddress);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(nameof(SelectAddressForServiceOfNotices), model);
            }

            var buttonAction = "SaveAndContinue";

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.SelectAddressForServiceOfNotices);

            await SaveSession(session, PagePaths.SelectAddressForServiceOfNotices, PagePaths.RegistrationLanding);

            await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueSelectAddressForServiceOfNoticesKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.RegistrationLanding);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }


        [HttpGet]
        [Route(PagePaths.ManualAddressForReprocessingSite)]
        public async Task<IActionResult> ManualAddressForReprocessingSite()
        {
            var model = GetStubDataFromTempData<ManualAddressForReprocessingSiteViewModel>(SaveAndContinueManualAddressForReprocessingSiteKey)
                        ?? new ManualAddressForReprocessingSiteViewModel();

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.ManualAddressForReprocessingSite };

            SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

            await SaveSession(session, PagePaths.ManualAddressForReprocessingSite, PagePaths.AddressForNotices);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.ManualAddressForReprocessingSite))
            {
                model = JsonConvert.DeserializeObject<ManualAddressForReprocessingSiteViewModel>(saveAndContinue.Parameters);
            }

            return View(nameof(ManualAddressForReprocessingSite), model);
        }

        [HttpPost]
        [Route(PagePaths.ManualAddressForReprocessingSite)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualAddressForReprocessingSite(ManualAddressForReprocessingSiteViewModel model, string buttonAction)
        {
            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForNotices, PagePaths.ManualAddressForReprocessingSite };

            SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

            await SaveSession(session, PagePaths.ManualAddressForReprocessingSite, PagePaths.AddressForNotices);

            await SaveAndContinue(0, nameof(ManualAddressForReprocessingSite), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueManualAddressForReprocessingSiteKey);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.AddressForNotices);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }

        [HttpGet]
        [Route(PagePaths.PostcodeForServiceOfNotices)]
        public async Task<IActionResult> PostcodeForServiceOfNotices()
        {
            var model = GetStubDataFromTempData<PostcodeForServiceOfNoticesViewModel>(SaveAndContinuePostcodeForServiceOfNoticesKey)
                        ?? new PostcodeForServiceOfNoticesViewModel();

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeForServiceOfNotices };

            SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

            await SaveSession(session, PagePaths.PostcodeForServiceOfNotices, PagePaths.RegistrationLanding);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.PostcodeForServiceOfNotices))
            {
                model = JsonConvert.DeserializeObject<PostcodeForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
            }

            return View(nameof(PostcodeForServiceOfNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.PostcodeForServiceOfNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostcodeForServiceOfNotices(PostcodeForServiceOfNoticesViewModel model, string buttonAction)
        {
            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.PostcodeForServiceOfNotices };

            SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

            await SaveSession(session, PagePaths.PostcodeForServiceOfNotices, PagePaths.SelectAddressForServiceOfNotices);

            await SaveAndContinue(0, nameof(PostcodeForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinuePostcodeForServiceOfNoticesKey);

            return Redirect(PagePaths.SelectAddressForServiceOfNotices);
        }

        [HttpGet]
        [Route(PagePaths.AddressOfReprocessingSite)]
        public async Task<IActionResult> AddressOfReprocessingSite()
        {
            var model = GetStubDataFromTempData<AddressOfReprocessingSiteViewModel>(SaveAndContinueAddressOfReprocessingSiteKey)
                        ?? new AddressOfReprocessingSiteViewModel
                        {
                            SelectedOption = null,
                            BusinessAddress = new AddressViewModel
                            {
                                AddressLine1 = "23 Ruby Street",
                                AddressLine2 = string.Empty,
                                TownOrCity = "London",
                                County = "Greater London",
                                Postcode = "EE12 345"
                            },
                            RegisteredAddress = null
                        };

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite };

            SetBackLink(session, PagePaths.AddressOfReprocessingSite);

            await SaveSession(session, PagePaths.AddressOfReprocessingSite, PagePaths.RegistrationLanding);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.AddressOfReprocessingSite))
            {
                model = JsonConvert.DeserializeObject<AddressOfReprocessingSiteViewModel>(saveAndContinue.Parameters);
            }

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.AddressOfReprocessingSite)]
        public async Task<IActionResult> AddressOfReprocessingSite(AddressOfReprocessingSiteViewModel model)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite };

            SetBackLink(session, PagePaths.AddressOfReprocessingSite);

            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            await SaveSession(session, PagePaths.AddressOfReprocessingSite, PagePaths.RegistrationLanding);

            await SaveAndContinue(0, nameof(AddressOfReprocessingSite), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueAddressOfReprocessingSiteKey);

            return Redirect(PagePaths.CountryOfReprocessingSite);
        }

        [HttpGet]
        [Route(PagePaths.CheckAnswers)]
        public async Task<IActionResult> CheckAnswers()
        {
            var model = GetStubDataFromTempData<CheckAnswersViewModel>(nameof(CheckAnswers))
                        ?? new CheckAnswersViewModel
                        {
                            SiteLocation = UkNation.England,
                            ReprocessingSiteAddress = new AddressViewModel
                            {
                                AddressLine1 = "2 Rhyl Coast Road",
                                AddressLine2 = string.Empty,
                                TownOrCity = "Rhyl",
                                County = "Denbighshire",
                                Postcode = "SE23 6FH"
                            },
                            SiteGridReference = "AB1234567890",
                            ServiceOfNoticesAddress = new AddressViewModel
                            {
                                AddressLine1 = "10 Rhyl Coast Road",
                                AddressLine2 = string.Empty,
                                TownOrCity = "Rhyl",
                                County = "Denbighshire",
                                Postcode = "SE23 6FH"
                            }
                        };

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.CheckAnswers };

            await SaveSession(session, PagePaths.CheckAnswers, PagePaths.RegistrationLanding);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.CheckAnswers))
            {
                model = JsonConvert.DeserializeObject<CheckAnswersViewModel>(saveAndContinue.Parameters);
            }

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.CheckAnswers)]
        public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.CheckAnswers };

            SetBackLink(session, PagePaths.CheckAnswers);

            await SaveSession(session, PagePaths.CheckAnswers, PagePaths.RegistrationLanding);

            await SaveAndContinue(0, nameof(CheckAnswers), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), nameof(CheckAnswers));

            // TODO
            var registrationId = 1;
            var updateRegistrationSiteAddressDto = _mapper.Map<UpdateRegistrationSiteAddressDto>(model);
            updateRegistrationSiteAddressDto.LegalDocumentAddress.NationId = (int)model.SiteLocation;
            updateRegistrationSiteAddressDto.LegalDocumentAddress.GridReference = model.SiteGridReference;
            updateRegistrationSiteAddressDto.LegalDocumentAddress.Country = "UK";

            updateRegistrationSiteAddressDto.ReprocessingSiteAddress.NationId = (int)model.SiteLocation;
            updateRegistrationSiteAddressDto.ReprocessingSiteAddress.GridReference = model.SiteGridReference;
            updateRegistrationSiteAddressDto.ReprocessingSiteAddress.Country = "UK";

            var updateRegistrationTaskStatusDto = new UpdateRegistrationTaskStatusDto
            {
                TaskName = RegistrationTaskType.SiteAddressAndContactDetails.ToString(),
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


            return Redirect(PagePaths.RegistrationLanding);
        }


        [HttpGet]
        [Route(PagePaths.SelectAddressForReprocessingSite)]
        public async Task<IActionResult> SelectAddressForReprocessingSite()
        {

            var viewModel = GetStubDataFromTempData<SelectAddressForReprocessingSiteViewModel>(nameof(SelectAddressForReprocessingSite))
                        ?? new SelectAddressForReprocessingSiteViewModel();

            // TEMP 
            if (viewModel.Addresses?.Count == 0)
            {
                viewModel.Postcode = "G2 0US";
                viewModel.SelectedIndex = null;
                viewModel.Addresses = GetListOfAddresses(viewModel.Postcode);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForReprocessingSite };

            SetBackLink(session, PagePaths.SelectAddressForReprocessingSite);

            await SaveSession(session, PagePaths.SelectAddressForReprocessingSite, PagePaths.RegistrationLanding);

            // check save and continue data
            var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
            if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.SelectAddressForReprocessingSite))
            {
                viewModel = JsonConvert.DeserializeObject<SelectAddressForReprocessingSiteViewModel>(saveAndContinue.Parameters);
            }

            return View(nameof(SelectAddressForReprocessingSite), viewModel);
        }

        [HttpGet]
        [Route(PagePaths.SelectedAddressForReprocessingSite)]
        public async Task<IActionResult> SelectedAddressForReprocessingSite([FromQuery] SelectedAddressViewModel selectedAddress)
        {
            var model = GetStubDataFromTempData<SelectAddressForReprocessingSiteViewModel>(nameof(SelectAddressForReprocessingSite))
                        ?? new SelectAddressForReprocessingSiteViewModel();

            model.SelectedIndex = selectedAddress.SelectedIndex;

            // TEMP 
            if (model.Addresses?.Count == 0)
            {
                model.Postcode = string.IsNullOrWhiteSpace(selectedAddress.Postcode) ? "G5 0US" : selectedAddress.Postcode;
                model.Addresses = GetListOfAddresses(model.Postcode);
            }

            var validationResult = await _validationService.ValidateAsync(selectedAddress);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(nameof(SelectAddressForReprocessingSite), model);
            }

            var buttonAction = "SaveAndContinue";

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForReprocessingSite };

            SetBackLink(session, PagePaths.SelectAddressForReprocessingSite);

            await SaveSession(session, PagePaths.SelectAddressForReprocessingSite, PagePaths.RegistrationLanding);

            await SaveAndContinue(0, nameof(ManualAddressForReprocessingSite), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), nameof(SelectAddressForReprocessingSite));

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.GridReferenceOfReprocessingSite);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }


        [HttpGet($"{PagePaths.RegistrationLanding}{PagePaths.ApplicationSaved}", Name = RegistrationRouteIds.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        [HttpGet(PagePaths.ConfirmNoticesAddress)]
        public IActionResult ConfirmNoticesAddress()
        {
            var model = new ConfirmNoticesAddressViewModel();
            SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);
            return View(model);
        }

        [HttpPost(PagePaths.ConfirmNoticesAddress)]
        public IActionResult ConfirmNoticesAddress(ConfirmNoticesAddressViewModel model)
        {
            SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);
            return View(model);
        }

        [HttpGet(PagePaths.PermitForRecycleWaste)]
        public IActionResult SelectAuthorisationType(string? nationCode = null)
        {
            var model = new SelectAuthorisationTypeViewModel();
            model.NationCode = nationCode;
            model.AuthorisationTypes = GetAuthorisationTypes(nationCode);

            SetTempBackLink(PagePaths.RegistrationLanding, PagePaths.PermitForRecycleWaste);
            return View(model);
        }

        [HttpPost(PagePaths.PermitForRecycleWaste)]
        public IActionResult SelectAuthorisationType(SelectAuthorisationTypeViewModel model, string buttonAction)
        {
            SetTempBackLink(PagePaths.RegistrationLanding, PagePaths.PermitForRecycleWaste);

            var selectedText = model.AuthorisationTypes.FirstOrDefault(x => x.Id == model.SelectedAuthorisation)?.SelectedAuthorisationText;
            var hasData = !string.IsNullOrEmpty(selectedText);
            string message = string.Empty;

            switch (model.SelectedAuthorisation)
            {
                case 1 when !hasData:
                    message = _selectAuthorisationStringLocalizer["error_message_enter_permit_or_license_number"];
                    ModelState.AddModelError($"AuthorisationTypes.SelectedAuthorisationText[{model.SelectedAuthorisation - 1}]", message);
                    break;
                case 2 or 3 or 4 when !hasData:
                    message = _selectAuthorisationStringLocalizer["error_message_enter_permit_number"];
                    ModelState.AddModelError($"AuthorisationTypes.SelectedAuthorisationText[{model.SelectedAuthorisation - 1}]", message);
                    break;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.RegistrationLanding, PagePaths.ApplicationSaved);
        }

        [HttpGet(PagePaths.WasteManagementLicense)]
        public IActionResult ProvideWasteManagementLicense()
        {
            var model = new MaterialPermitViewModel
            {
                MaterialType = MaterialType.Licence
            };
            SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense);
            return View(model);
        }

        [HttpPost(PagePaths.WasteManagementLicense)]
        public IActionResult ProvideWasteManagementLicense(MaterialPermitViewModel model, string buttonAction)
        {
            SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense);

            if (!ModelState.IsValid) return View(model);

            return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.RegistrationLanding, PagePaths.ApplicationSaved);
        }

        [HttpGet]
        [Route(PagePaths.ExemptionReferences)]
        public async Task<IActionResult> ExemptionReferences()
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.ExemptionReferences);

            return View(nameof(ExemptionReferences), new ExemptionReferencesViewModel());
        }

        [HttpPost]
        [Route(PagePaths.ExemptionReferences)]
        public async Task<IActionResult> ExemptionReferences(ExemptionReferencesViewModel viewModel, string buttonAction)
        {
            await SetTempBackLink(PagePaths.PermitForRecycleWaste, PagePaths.ExemptionReferences);

            if (!ModelState.IsValid)
            {
                return View(nameof(ExemptionReferences), viewModel);
            }

            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();

            await SaveSession(session, PagePaths.ExemptionReferences, PagePaths.PpcPermit);

            await SaveAndContinue(0, nameof(ExemptionReferences), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(viewModel), SaveAndContinuePostcodeForServiceOfNoticesKey);


            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.PpcPermit);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(ExemptionReferences), viewModel);
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
        #endregion
    }
}