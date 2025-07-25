﻿using System.Globalization;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.Services;
using Address = Epr.Reprocessor.Exporter.UI.App.Domain.Address;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.RegistrationLanding)]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class RegistrationController : RegistrationControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;

        public static class RegistrationRouteIds
        {
            public const string ApplicationSaved = "registration.application-saved";
            public const string Confirmation = "registration.confirmation";
        }

        public RegistrationController(
            ILogger<RegistrationController> logger,
            ISessionManager<ReprocessorRegistrationSession> sessionManager,
            IReprocessorService reprocessorService,
            IPostcodeLookupService postcodeLookupService,
            IValidationService validationService,
            IRequestMapper requestMapper)
            : base(sessionManager, reprocessorService, postcodeLookupService,
            validationService, requestMapper)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route(PagePaths.Placeholder)]
        public IActionResult Placeholder()
        {
            return View(nameof(Placeholder));
        }

        [HttpPost]
        [Route(PagePaths.MaximumWeightSiteCanReprocess)]
        public async Task<IActionResult> MaximumWeightSiteCanReprocess(MaximumWeightSiteCanReprocessViewModel viewModel, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.MaximumWeightSiteCanReprocess];

            SetBackLink(session, PagePaths.MaximumWeightSiteCanReprocess);

            if (!ModelState.IsValid)
            {
                return View(nameof(MaximumWeightSiteCanReprocess), viewModel);
            }

            await SaveSession(session, PagePaths.MaximumWeightSiteCanReprocess);

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
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence];

            SetBackLink(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            if (!ModelState.IsValid)
            {
                return View(nameof(EnvironmentalPermitOrWasteManagementLicence), viewModel);
            }

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;
            var capacityInTonnes = decimal.Parse(viewModel.MaximumWeight!);
            var selectedFrequency = (int)viewModel.SelectedFrequency!;

            wasteDetails!.SetEnvironmentalPermitOrWasteManagementLicence(capacityInTonnes, selectedFrequency);

            await SaveSession(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            var registrationMaterialId = wasteDetails.CurrentMaterialApplyingFor.Id;

            if (registrationMaterialId != Guid.Empty)
            {
                var dto = new UpdateRegistrationMaterialPermitCapacityDto
                {
                    PermitTypeId = (int)wasteDetails.CurrentMaterialApplyingFor.PermitType,
                    CapacityInTonnes = capacityInTonnes,
                    PeriodId = selectedFrequency,
                };

                await ReprocessorService
                    .RegistrationMaterials
                    .UpdateRegistrationMaterialPermitCapacityAsync(registrationMaterialId, dto);
            }

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.MaximumWeightSiteCanReprocess);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(EnvironmentalPermitOrWasteManagementLicence), viewModel);
        }

        [HttpGet]
        [Route(PagePaths.EnvironmentalPermitOrWasteManagementLicence)]
        public async Task<IActionResult> EnvironmentalPermitOrWasteManagementLicence()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence];

            SetBackLink(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails?.CurrentMaterialApplyingFor is null)
            {
                return RedirectToAction(nameof(WastePermitExemptions));
            }

            var model = new MaterialPermitViewModel
            {
                MaterialType = MaterialType.Permit,
                Material = wasteDetails.CurrentMaterialApplyingFor.Name.GetMaterialName()
            };

            if (wasteDetails.CurrentMaterialApplyingFor.PermitType is PermitType.EnvironmentalPermitOrWasteManagementLicence)
            {
                var currentMaterial = wasteDetails.CurrentMaterialApplyingFor;

                model.MaximumWeight = currentMaterial.WeightInTonnes.ToStringWithOutDecimalPlaces();
                model.SelectedFrequency = (PermitPeriod?)(int?)currentMaterial.PermitPeriod;
            }

            return View(nameof(EnvironmentalPermitOrWasteManagementLicence), model);
        }

        [HttpPost]
        [Route(PagePaths.InstallationPermit)]
        public async Task<IActionResult> InstallationPermit(MaterialPermitViewModel viewModel, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.InstallationPermit];

            SetBackLink(session, PagePaths.InstallationPermit);

            if (!ModelState.IsValid)
            {
                return View(nameof(InstallationPermit), viewModel);
            }

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            var capacityInTonnes = decimal.Parse(viewModel.MaximumWeight!);
            var selectedFrequency = (int)viewModel.SelectedFrequency!;

            wasteDetails!.SetInstallationPermit(capacityInTonnes, selectedFrequency);

            await SaveSession(session, PagePaths.InstallationPermit);

            if (wasteDetails.RegistrationMaterialId.HasValue)
            {
                var dto = new UpdateRegistrationMaterialPermitCapacityDto
                {
                    PermitTypeId = (int)wasteDetails.CurrentMaterialApplyingFor.PermitType,
                    CapacityInTonnes = capacityInTonnes,
                    PeriodId = selectedFrequency,
                };

                await ReprocessorService
                    .RegistrationMaterials
                    .UpdateRegistrationMaterialPermitCapacityAsync(wasteDetails.RegistrationMaterialId.Value, dto);
            }

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.MaximumWeightSiteCanReprocess);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(InstallationPermit), viewModel);

        }

        [HttpGet]
        [Route(PagePaths.InstallationPermit)]
        public async Task<IActionResult> InstallationPermit()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.InstallationPermit];

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails?.CurrentMaterialApplyingFor is null)
            {
                return RedirectToAction(nameof(WastePermitExemptions));
            }

            var model = new MaterialPermitViewModel
            {
                MaterialType = MaterialType.Permit,
                Material = wasteDetails.CurrentMaterialApplyingFor.Name.GetMaterialName()
            };

            if (wasteDetails.CurrentMaterialApplyingFor!.PermitType is PermitType.InstallationPermit)
            {
                var currentMaterial = wasteDetails.CurrentMaterialApplyingFor;

                model.MaximumWeight = currentMaterial.WeightInTonnes.ToString(CultureInfo.InvariantCulture);
                model.SelectedFrequency = (PermitPeriod?)(int?)currentMaterial.PermitPeriod;
            }

            SetBackLink(session, PagePaths.InstallationPermit);

            return View(nameof(InstallationPermit), model);
        }

        [HttpPost]
        [Route(PagePaths.PpcPermit)]
        public async Task<IActionResult> PpcPermit(MaterialPermitViewModel viewModel, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.PpcPermit];

            SetBackLink(session, PagePaths.PpcPermit);

            if (!ModelState.IsValid)
            {
                return View(nameof(PpcPermit), viewModel);
            }

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            var capacityInTonnes = decimal.Parse(viewModel.MaximumWeight!);
            var selectedFrequency = (int)viewModel.SelectedFrequency!;

            wasteDetails!.SetPPCPermit(capacityInTonnes, selectedFrequency);

            await SaveSession(session, PagePaths.PpcPermit);

            var registrationMaterialId = wasteDetails.CurrentMaterialApplyingFor.Id;

            if (registrationMaterialId != Guid.Empty)
            {
                var dto = new UpdateRegistrationMaterialPermitCapacityDto
                {
                    PermitTypeId = (int)wasteDetails.CurrentMaterialApplyingFor.PermitType,
                    CapacityInTonnes = capacityInTonnes,
                    PeriodId = selectedFrequency,
                };

                await ReprocessorService
                    .RegistrationMaterials
                    .UpdateRegistrationMaterialPermitCapacityAsync(registrationMaterialId, dto);

            }               

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.MaximumWeightSiteCanReprocess);
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
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.PpcPermit];

            SetBackLink(session, PagePaths.PpcPermit);

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails?.CurrentMaterialApplyingFor is null)
            {
                return RedirectToAction(nameof(WastePermitExemptions));
            }

            var model = new MaterialPermitViewModel
            {
                MaterialType = MaterialType.Permit,
                Material = wasteDetails.CurrentMaterialApplyingFor.Name.GetMaterialName()
            };

            if (wasteDetails.CurrentMaterialApplyingFor.PermitType is PermitType.PollutionPreventionAndControlPermit)
            {
                var currentMaterial = wasteDetails.CurrentMaterialApplyingFor;

                model.MaximumWeight = currentMaterial.WeightInTonnes.ToString(CultureInfo.InvariantCulture);
                model.SelectedFrequency = (PermitPeriod?)(int?)currentMaterial.PermitPeriod;
            }

            return View(nameof(PpcPermit), model);
        }

        [HttpGet]
        [Route(PagePaths.WastePermitExemptions)]
        public async Task<IActionResult> WastePermitExemptions([FromServices]IModelFactory<WastePermitExemptionsViewModel> modelFactory)
        {
            var model = modelFactory.Instance;

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.TaskList, PagePaths.WastePermitExemptions];

            if (session.RegistrationId is null)
            {
                var registration = await ReprocessorService.Registrations.GetByOrganisationAsync(1, HttpContext.User.GetOrganisationId()!.Value);

                if (registration is null)
                {
                    return Redirect(PagePaths.TaskList);
                }

                session.RegistrationId = registration.Id;

                await SaveSession(session, PagePaths.WastePermitExemptions);
            }

            SetBackLink(session, PagePaths.WastePermitExemptions);


            // We do not have a list of applicable materials in session, retrieve from backend and load them to the UI.
            var allApplicableMaterials = await ReprocessorService.Materials.GetAllMaterialsAsync();
            model.MapForView(allApplicableMaterials);

            // Check if there is any existing registration materials.
            var registrationId = session.RegistrationId;
            var existingRegistrationMaterials = await ReprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsAsync(registrationId!.Value);

            if (existingRegistrationMaterials.Count > 0)
            {
                // For any registration material that already has been either registered or started to be registered previously, ensure their 
                // corresponding checkbox is checked on the UI.
                // This also sets the selected materials in the model accordingly.
                model.SetExistingMaterialsAsChecked(existingRegistrationMaterials.Select(o => o.Name).ToList());
            }

            // We always want to do this to ensure we have the up-to-date entries.
            session.RegistrationApplicationSession.WasteDetails!.SetFromExisting(existingRegistrationMaterials);

            await SaveSession(session, PagePaths.WastePermitExemptions);

            return View(nameof(WastePermitExemptions), model);
        }

        [HttpPost]
        [Route(PagePaths.WastePermitExemptions)]
        public async Task<IActionResult> WastePermitExemptions(WastePermitExemptionsViewModel model,
            string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.TaskList, PagePaths.WastePermitExemptions];

            SetBackLink(session, PagePaths.WastePermitExemptions);

            if (!ModelState.IsValid)
            {
                // We do not have a list of applicable materials in session, retrieve from backend and load them to the UI.
                var allApplicableMaterials = await ReprocessorService.Materials.GetAllMaterialsAsync();
                model.MapForView(allApplicableMaterials);
                return View(nameof(WastePermitExemptions), model);
            }

            var preExistingSelectedMaterials = session.RegistrationApplicationSession.WasteDetails!.SelectedMaterials;
            var proposedSelectedMaterials = model.SelectedMaterials;

            var materialsToRemove = preExistingSelectedMaterials.ExceptBy(proposedSelectedMaterials, o => o.Name.ToString()).ToList();

            if (materialsToRemove.Count > 0)
            {
                foreach (var material in materialsToRemove.Select(o => o.Id))
                {
                    await ReprocessorService.RegistrationMaterials.DeleteAsync(material);
                    session.RegistrationApplicationSession.WasteDetails!.SelectedMaterials.RemoveAll(o => o.Id == material);
                }
            }

            proposedSelectedMaterials = proposedSelectedMaterials.Where(o => preExistingSelectedMaterials.Find(x => x.Name.ToString() == o) is null).ToList();
            foreach (var item in proposedSelectedMaterials)
            {
                var request = new CreateRegistrationMaterialDto
                {
                    RegistrationId = session.RegistrationId!.Value,
                    Material = Enum.Parse<Material>(item)
                };

                var created = await ReprocessorService.RegistrationMaterials.CreateAsync(request);
                if (created is not null)
                {
                    session.RegistrationApplicationSession.RegistrationTasks.SetTaskAsInProgress(TaskType.WasteLicensesPermitsExemptions);
                    session.RegistrationApplicationSession.WasteDetails!.RegistrationMaterialCreated(created);
                }
            }
            
            await SaveSession(session, PagePaths.WastePermitExemptions);

            return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.PermitForRecycleWaste, PagePaths.ApplicationSaved);
        }

        [HttpGet]
        [Route(PagePaths.AddressForNotices)]
        public async Task<IActionResult> AddressForNotices()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
            session.Journey = [reprocessingSite!.SourcePage, PagePaths.AddressForNotices];

            SetBackLink(session, PagePaths.AddressForNotices);

            var organisation = HttpContext.GetUserData().Organisations.FirstOrDefault();

            if (organisation is null)
            {
                throw new ArgumentNullException(nameof(organisation));
            }

            if (organisation.NationId is 0 or null)
            {
                return Redirect(PagePaths.CountryOfReprocessingSite);
            }

            var model = new AddressForNoticesViewModel
            {
                IsBusinessAddress = string.IsNullOrEmpty(organisation.CompaniesHouseNumber),
                BusinessAddress = new AddressViewModel
                {
                    AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                    AddressLine2 = organisation.Locality,
                    TownOrCity = organisation.Town ?? string.Empty,
                    County = organisation.County ?? string.Empty,
                    Postcode = organisation.Postcode ?? string.Empty
                },
                SiteAddress = new AddressViewModel
                {
                    AddressLine1 = reprocessingSite.Address?.AddressLine1 ?? string.Empty,
                    AddressLine2 = reprocessingSite.Address?.AddressLine2 ?? string.Empty,
                    TownOrCity = reprocessingSite.Address?.Town ?? string.Empty,
                    County = reprocessingSite.Address?.County ?? string.Empty,
                    Postcode = reprocessingSite.Address?.Postcode ?? string.Empty
                },
                ShowSiteAddress = reprocessingSite.TypeOfAddress == AddressOptions.DifferentAddress
            };

            await SaveSession(session, PagePaths.AddressForNotices);

            return View(nameof(AddressForNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.AddressForNotices)]
        public async Task<IActionResult> AddressForNotices(AddressForNoticesViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
            session.Journey = [reprocessingSite!.SourcePage, PagePaths.AddressForNotices];

            SetBackLink(session, PagePaths.AddressForNotices);

            var validationResult = await ValidationService.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            if (model.SelectedAddressOptions is AddressOptions.DifferentAddress)
            {
                reprocessingSite.ServiceOfNotice!.TypeOfAddress = AddressOptions.DifferentAddress;
            }
            else
            {
                reprocessingSite.ServiceOfNotice!.SetAddress(model.GetAddress(), model.SelectedAddressOptions);
            }

            await SaveSession(session, PagePaths.AddressForNotices);

            return Redirect(model.SelectedAddressOptions is AddressOptions.DifferentAddress ? PagePaths.PostcodeForServiceOfNotices : PagePaths.CheckAnswers);
        }

        [HttpGet]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<IActionResult> UKSiteLocation()
        {
            var model = new UKSiteLocationViewModel();
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.AddressOfReprocessingSite, PagePaths.CountryOfReprocessingSite];
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

            if (reprocessingSite != null)
            {
                model.SiteLocationId = reprocessingSite.Nation;
            }

            SetBackLink(session, PagePaths.CountryOfReprocessingSite);

            await SaveSession(session, PagePaths.AddressOfReprocessingSite);

            return View(nameof(UKSiteLocation), model);
        }

        [HttpPost]
        [Route(PagePaths.CountryOfReprocessingSite)]
        public async Task<ActionResult> UKSiteLocation(UKSiteLocationViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.RegistrationLanding, PagePaths.CountryOfReprocessingSite];

            await SetTempBackLink(PagePaths.AddressForNotices, PagePaths.CountryOfReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            session.RegistrationApplicationSession.ReprocessingSite?.SetNation((UkNation)model.SiteLocationId!);

            await SaveSession(session, PagePaths.AddressOfReprocessingSite);

            return Redirect(PagePaths.PostcodeOfReprocessingSite);
        }

        [HttpGet]
        [Route(PagePaths.NoAddressFound)]
        public async Task<IActionResult> NoAddressFound([FromQuery] AddressLookupType addressLookupType = AddressLookupType.ReprocessingSite)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();

            string previousPagePath;
            LookupAddress lookupAddress;

            switch (addressLookupType)
            {
                case AddressLookupType.LegalDocuments:
                    previousPagePath = PagePaths.PostcodeForServiceOfNotices;
                    lookupAddress = session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;
                    break;
                default:
                    previousPagePath = PagePaths.PostcodeOfReprocessingSite;
                    lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
                    break;
            }

            session.Journey = new List<string> { previousPagePath, PagePaths.NoAddressFound };

            SetBackLink(session, PagePaths.NoAddressFound);

            await SaveSession(session, PagePaths.NoAddressFound);

            var model = new NoAddressFoundViewModel
            {
                Postcode = lookupAddress?.Postcode,
                LookupType = addressLookupType
            };

            return View(model);
        }

        [HttpGet]
        [Route(PagePaths.PostcodeOfReprocessingSite)]
        public async Task<IActionResult> PostcodeOfReprocessingSite()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.CountryOfReprocessingSite, PagePaths.PostcodeOfReprocessingSite];

            SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);
            
            await SaveSession(session, PagePaths.PostcodeOfReprocessingSite);

            var sessionLookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
            var model = new PostcodeOfReprocessingSiteViewModel(sessionLookupAddress?.Postcode);

            return View(model);
        }

        [HttpGet]
        [Route(PagePaths.TaskList)]
        public async Task<IActionResult> TaskList()
        {
            var model = new TaskListModel();

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = ["/", PagePaths.TaskList];

            SetBackLink(session, PagePaths.TaskList);

            model.TaskList = session.RegistrationApplicationSession.RegistrationTasks.Items;

            await SaveSession(session, PagePaths.TaskList);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.PostcodeOfReprocessingSite)]
        public async Task<IActionResult> PostcodeOfReprocessingSite(PostcodeOfReprocessingSiteViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.CountryOfReprocessingSite, PagePaths.PostcodeOfReprocessingSite];

            SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var sessionLookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
            sessionLookupAddress.Postcode = model.Postcode;

            var addressList = await PostcodeLookupService.GetAddressListByPostcodeAsync(sessionLookupAddress.Postcode);
            var newLookupAddress = new LookupAddress(model.Postcode, addressList, sessionLookupAddress.SelectedAddressIndex);
            session.RegistrationApplicationSession.ReprocessingSite.LookupAddress = newLookupAddress;

            await SaveSession(session, PagePaths.PostcodeOfReprocessingSite);

            if (addressList is null || !addressList.Addresses.Any())
            {
                return RedirectToAction("NoAddressFound", new { addressLookupType = (int)AddressLookupType.ReprocessingSite });
            }

            return Redirect(PagePaths.SelectAddressForReprocessingSite);
        }

        [HttpGet]
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();

            session.Journey = [PagePaths.RegistrationLanding, PagePaths.GridReferenceForEnteredReprocessingSite];

            session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.GridReferenceForEnteredReprocessingSite);
            await SaveSession(session, PagePaths.GridReferenceForEnteredReprocessingSite);

            await SetTempBackLink(PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;

            var displayAddress = string.Empty;
            if (lookupAddress.SelectedAddress is not null)
            {
                var address = lookupAddress.SelectedAddress;
                displayAddress = string.Join(", ", new[] { address.AddressLine1, address.AddressLine2, address.Locality, address.Town, address.County, address.Postcode }
                                      .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));
            }
            var model = new ProvideSiteGridReferenceViewModel
            {
                Address = displayAddress.ToUpper(),
                GridReference = session.RegistrationApplicationSession.ReprocessingSite.SiteGridReference
            };

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.GridReferenceForEnteredReprocessingSite)]
        public async Task<IActionResult> ProvideSiteGridReference(ProvideSiteGridReferenceViewModel model, string buttonAction)
        {
            await SetTempBackLink(PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey =
                [PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite];

            session.RegistrationApplicationSession.ReprocessingSite!.SetSiteGridReference(model.GridReference);

            await SaveSession(session, PagePaths.GridReferenceForEnteredReprocessingSite);

            await CreateRegistrationIfNotExistsAsync();

            return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.AddressForNotices, PagePaths.ApplicationSaved);
        }

        [HttpGet]
        [Route(PagePaths.ManualAddressForServiceOfNotices)]
        public async Task<IActionResult> ManualAddressForServiceOfNotices()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

            session.Journey =
            [
                reprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.TaskList,
                PagePaths.ManualAddressForServiceOfNotices
            ];

            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            var model = new ManualAddressForServiceOfNoticesViewModel();
            var address = reprocessingSite.ServiceOfNotice?.Address;

            if (address is not null)
            {
                model = new ManualAddressForServiceOfNoticesViewModel
                {
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    County = address.County,
                    Postcode = address.Postcode,
                    TownOrCity = address.Town,
                };
            }

            await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices);

            return View(nameof(ManualAddressForServiceOfNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.ManualAddressForServiceOfNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualAddressForServiceOfNotices(ManualAddressForServiceOfNoticesViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

            session.Journey =
            [
                reprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.TaskList,
                PagePaths.ManualAddressForServiceOfNotices
            ];

            SetBackLink(session, PagePaths.ManualAddressForServiceOfNotices);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.SetAddress(model.GetAddress(), AddressOptions.DifferentAddress);

            await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices);

            IActionResult result = View(model);

            if (buttonAction == SaveAndContinueActionKey)
            {
                result = Redirect(PagePaths.CheckYourAnswersForContactDetails);
            }
            else if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                session.RegistrationApplicationSession.RegistrationTasks.SetTaskAsInProgress(TaskType.SiteAddressAndContactDetails);
                await SaveSession(session, PagePaths.ManualAddressForServiceOfNotices);

                result = Redirect(PagePaths.ApplicationSaved);
            }

            await UpdateRegistrationAsync();

            return result;
        }

        [HttpGet]
        [Route(PagePaths.GridReferenceOfReprocessingSite)]
        public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite()
        {
            var model = new ProvideGridReferenceOfReprocessingSiteViewModel();

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();

            session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.GridReferenceOfReprocessingSite);

            await SaveSession(session, PagePaths.GridReferenceOfReprocessingSite);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
            model.GridReference = session.RegistrationApplicationSession.ReprocessingSite.SiteGridReference;

            if (lookupAddress?.SelectedAddressIndex.HasValue is true)
            {
                await SetTempBackLink(PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite);
            }
            else
            {
                await SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);
            }

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.GridReferenceOfReprocessingSite)]
        public async Task<IActionResult> ProvideGridReferenceOfReprocessingSite(ProvideGridReferenceOfReprocessingSiteViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.RegistrationLanding, PagePaths.GridReferenceOfReprocessingSite];

            await SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

            if (!ModelState.IsValid)
            {
                return View(nameof(ProvideGridReferenceOfReprocessingSite), model);
            }

            session.RegistrationApplicationSession.ReprocessingSite!.SetSiteGridReference(model.GridReference);
            await SaveSession(session, PagePaths.GridReferenceOfReprocessingSite);

            await CreateRegistrationIfNotExistsAsync();

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.AddressForNotices);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(ProvideGridReferenceOfReprocessingSite), model);
        }

        [HttpGet]
        [Route(PagePaths.SelectAddressForServiceOfNotices)]
        public async Task<IActionResult> SelectAddressForServiceOfNotices(int? selectedIndex = null)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = new List<string> { PagePaths.PostcodeForServiceOfNotices, PagePaths.SelectAddressForServiceOfNotices };

            SetBackLink(session, PagePaths.SelectAddressForServiceOfNotices);

            session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.SetSourcePage(PagePaths.SelectAddressForServiceOfNotices);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;

            // Postback : Address selected
            var addressSelected = selectedIndex.HasValue && selectedIndex > -1 && selectedIndex < lookupAddress!.AddressesForPostcode.Count;
            if (addressSelected)
            {
                lookupAddress.SelectedAddressIndex = selectedIndex;
                session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.SetAddress(lookupAddress.SelectedAddress, AddressOptions.DifferentAddress);
            }

            await SaveSession(session, PagePaths.SelectAddressForServiceOfNotices);

            if (addressSelected)
            {
                return Redirect(PagePaths.ConfirmNoticesAddress);
            }

            var viewModel = new SelectAddressForServiceOfNoticesViewModel(lookupAddress);
            return View(nameof(SelectAddressForServiceOfNotices), viewModel);
        }


        [HttpGet]
        [Route(PagePaths.ManualAddressForReprocessingSite)]
        public async Task<IActionResult> ManualAddressForReprocessingSite()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

            if (reprocessingSite?.TypeOfAddress is null or not AddressOptions.DifferentAddress)
            {
                return Redirect(PagePaths.AddressOfReprocessingSite);
            }

            session.Journey = [reprocessingSite.SourcePage, PagePaths.ManualAddressForReprocessingSite];
            SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

            var model = new ManualAddressForReprocessingSiteViewModel();
            var address = reprocessingSite.Address;

            if (address is not null)
            {
                model = new ManualAddressForReprocessingSiteViewModel
                {
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    County = address.County,
                    Postcode = address.Postcode,
                    TownOrCity = address.Town,
                    SiteGridReference = reprocessingSite.SiteGridReference
                };
            }

            await SaveSession(session, PagePaths.ManualAddressForReprocessingSite);

            return View(nameof(ManualAddressForReprocessingSite), model);
        }

        [HttpPost]
        [Route(PagePaths.ManualAddressForReprocessingSite)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualAddressForReprocessingSite(ManualAddressForReprocessingSiteViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

            session.Journey = [reprocessingSite!.SourcePage, PagePaths.ManualAddressForReprocessingSite];

            SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var country = reprocessingSite.Nation?.GetDisplayName();
            session.RegistrationApplicationSession.ReprocessingSite?.SetAddress(
                new Address(model.AddressLine1,
                    model.AddressLine2,
                    null,
                    model.TownOrCity,
                    model.County,
                    country,
                    model.Postcode),
                AddressOptions.DifferentAddress);

            session.RegistrationApplicationSession.ReprocessingSite?.SetSiteGridReference(model.SiteGridReference);
            await SaveSession(session, PagePaths.ManualAddressForReprocessingSite);

            await CreateRegistrationIfNotExistsAsync();

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.AddressForNotices);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }

        [HttpGet]
        [Route(PagePaths.PostcodeForServiceOfNotices)]
        public async Task<IActionResult> PostcodeForServiceOfNotices()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForNotices, PagePaths.PostcodeForServiceOfNotices };

            SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

            session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.SetSourcePage(PagePaths
                .PostcodeForServiceOfNotices);

            await SaveSession(session, PagePaths.PostcodeForServiceOfNotices);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.LookupAddress;
            var model = new PostcodeForServiceOfNoticesViewModel(lookupAddress?.Postcode);

            return View(nameof(PostcodeForServiceOfNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.PostcodeForServiceOfNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostcodeForServiceOfNotices(PostcodeForServiceOfNoticesViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.AddressForNotices, PagePaths.PostcodeForServiceOfNotices];

            SetBackLink(session, PagePaths.PostcodeForServiceOfNotices);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            var sessionLookupAddress = session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;
            sessionLookupAddress!.Postcode = model.Postcode;

            var addressList = await PostcodeLookupService.GetAddressListByPostcodeAsync(sessionLookupAddress.Postcode!);
            var newLookupAddress = new LookupAddress(model.Postcode, addressList, sessionLookupAddress.SelectedAddressIndex);
            session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.LookupAddress = newLookupAddress;

            await SaveSession(session, PagePaths.PostcodeForServiceOfNotices);

            if (addressList is null || !addressList.Addresses.Any())
            {
                return RedirectToAction("NoAddressFound", new { addressLookupType = (int)AddressLookupType.LegalDocuments });
            }

            return Redirect(PagePaths.SelectAddressForServiceOfNotices);
        }

        [HttpGet]
        [Route(PagePaths.AddressOfReprocessingSite)]
        public async Task<IActionResult> AddressOfReprocessingSite()
        {
            var model = new AddressOfReprocessingSiteViewModel();

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite];
            SetBackLink(session, PagePaths.AddressOfReprocessingSite);
            session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.AddressOfReprocessingSite);

            var organisation = HttpContext.GetUserData().Organisations.FirstOrDefault();

            if (organisation is null)
            {
                throw new ArgumentNullException(nameof(organisation));
            }

            if (organisation.NationId is 0 or null)
            {
                return Redirect(PagePaths.CountryOfReprocessingSite);
            }

            // Not a companies house organisation.
            if (string.IsNullOrEmpty(organisation.CompaniesHouseNumber))
            {
                model = new AddressOfReprocessingSiteViewModel
                {
                    SelectedOption = session.RegistrationApplicationSession?.ReprocessingSite?.TypeOfAddress,
                    RegisteredAddress = null,
                    BusinessAddress = new AddressViewModel
                    {
                        AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                        AddressLine2 = organisation.Locality,
                        TownOrCity = organisation.Town ?? string.Empty,
                        County = organisation.County ?? string.Empty,
                        Postcode = organisation.Postcode ?? string.Empty
                    }
                };
            }
            // Is a companies house organisation.
            else
            {
                model = new AddressOfReprocessingSiteViewModel
                {
                    SelectedOption = session.RegistrationApplicationSession?.ReprocessingSite?.TypeOfAddress,
                    BusinessAddress = null,
                    RegisteredAddress = new AddressViewModel
                    {
                        AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                        AddressLine2 = organisation.Locality,
                        TownOrCity = organisation.Town ?? string.Empty,
                        County = organisation.County ?? string.Empty,
                        Postcode = organisation.Postcode ?? string.Empty
                    }
                };
            }
            
            await SaveSession(session, PagePaths.AddressOfReprocessingSite);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.AddressOfReprocessingSite)]
        public async Task<IActionResult> AddressOfReprocessingSite(AddressOfReprocessingSiteViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite];

            SetBackLink(session, PagePaths.AddressOfReprocessingSite);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            session.RegistrationApplicationSession.ReprocessingSite!.SetAddress(model.GetAddress(), model.SelectedOption);

            await SaveSession(session, PagePaths.AddressOfReprocessingSite);

            return Redirect(model.SelectedOption is AddressOptions.RegisteredAddress or AddressOptions.BusinessAddress ?
                PagePaths.GridReferenceOfReprocessingSite : PagePaths.CountryOfReprocessingSite);
        }

        [HttpGet]
        [Route(PagePaths.CheckAnswers)]
        public async Task<IActionResult> CheckAnswers()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForNotices, PagePaths.CheckAnswers };

            SetBackLink(session, PagePaths.CheckAnswers);

            await SaveSession(session, PagePaths.CheckAnswers);

            var model = new CheckAnswersViewModel(session.RegistrationApplicationSession.ReprocessingSite!);

            return View(model);
        }

        [HttpPost]
        [Route(PagePaths.CheckAnswers)]
        public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = new List<string> { PagePaths.ConfirmNoticesAddress, PagePaths.CheckAnswers };

            SetBackLink(session, PagePaths.CheckAnswers);

            await SaveSession(session, PagePaths.CheckAnswers);

            // Mark task status as completed
            await MarkTaskStatusAsCompleted(TaskType.SiteAddressAndContactDetails);

            return Redirect(PagePaths.ApplicationSaved);
        }


        [HttpGet]
        [Route(PagePaths.SelectAddressForReprocessingSite)]
        public async Task<IActionResult> SelectAddressForReprocessingSite(int? selectedIndex = null)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = new List<string> { PagePaths.PostcodeOfReprocessingSite, PagePaths.SelectAddressForReprocessingSite };

            SetBackLink(session, PagePaths.SelectAddressForReprocessingSite);

            session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.SelectAddressForReprocessingSite);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;

            // Postback : Address selected
            var addressSelected = selectedIndex.HasValue && selectedIndex > -1 && selectedIndex < lookupAddress.AddressesForPostcode.Count;
            if (addressSelected)
            {
                lookupAddress.SelectedAddressIndex = selectedIndex;
                session.RegistrationApplicationSession.ReprocessingSite!.SetAddress(lookupAddress.SelectedAddress, AddressOptions.DifferentAddress);
            }

            await SaveSession(session, PagePaths.SelectAddressForReprocessingSite);

            if (addressSelected)
            {
                return Redirect(PagePaths.GridReferenceForEnteredReprocessingSite);
            }

            var viewModel = new SelectAddressForReprocessingSiteViewModel(lookupAddress);

            return View(nameof(SelectAddressForReprocessingSite), viewModel);
        }

        [HttpGet(PagePaths.ApplicationSaved, Name = RegistrationRouteIds.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        [HttpGet(PagePaths.ConfirmNoticesAddress)]
        public async Task<IActionResult> ConfirmNoticesAddress()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();

            await SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);

            session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.SetSourcePage(PagePaths
                .ConfirmNoticesAddress);

            await SaveSession(session, PagePaths.ConfirmNoticesAddress);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;

            var displayAddress = string.Empty;
            if (lookupAddress.SelectedAddress is not null)
            {
                var address = lookupAddress.SelectedAddress;
                displayAddress = string.Join(", ", new[] { address.AddressLine1, address.AddressLine2, address.Locality, address.Town, address.County, address.Postcode }
                                      .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));
            }

            var viewModel = new ConfirmNoticesAddressViewModel
            {
                ConfirmAddress = displayAddress
            };

            return View(viewModel);
        }

        [HttpPost(PagePaths.ConfirmNoticesAddress)]
        public async Task<IActionResult> ConfirmNoticesAddress(ConfirmNoticesAddressViewModel model)
        {
            await SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);

            return Redirect(PagePaths.CheckAnswers);
        }

        [HttpGet(PagePaths.PermitForRecycleWaste)]
        public async Task<IActionResult> SelectAuthorisationType([FromServices]INationAccessor nationAccessor)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.WastePermitExemptions, PagePaths.PermitForRecycleWaste];

            SetBackLink(session, PagePaths.PermitForRecycleWaste);

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails?.CurrentMaterialApplyingFor is null)
            {
                return Redirect(PagePaths.WastePermitExemptions);
            }

            var nation = await nationAccessor.GetNation();

            var permitTypes = await ReprocessorService
                .RegistrationMaterials
                .GetMaterialsPermitTypesAsync();

            var authorisationTypes = await RequestMapper.MapAuthorisationTypes(permitTypes, nation.ToString());

            if (wasteDetails.CurrentMaterialApplyingFor.PermitType is not null && 
                wasteDetails.CurrentMaterialApplyingFor.PermitType is not PermitType.WasteExemption)
            {
                var matched = authorisationTypes.Find(o => o.Id == (int?)wasteDetails.CurrentMaterialApplyingFor.PermitType);
                var isCurrentPermitTypeAllowedForNation = matched is not null;

                if (!isCurrentPermitTypeAllowedForNation)
                {
                    authorisationTypes.ForEach(o => o.SelectedAuthorisationText = null);
                    wasteDetails.CurrentMaterialApplyingFor.ResetPermitDetails();
                }
                else
                {
                    matched!.SelectedAuthorisationText = wasteDetails.CurrentMaterialApplyingFor.PermitNumber;
                }
            }

            var model = new SelectAuthorisationTypeViewModel
            {
                NationCode = nation.ToString(),
                SelectedMaterial = wasteDetails.CurrentMaterialApplyingFor!.Name,
                AuthorisationTypes = authorisationTypes,
                SelectedAuthorisation = (int?)wasteDetails.CurrentMaterialApplyingFor!.PermitType,
            };

            await SaveSession(session, PagePaths.PermitForRecycleWaste);

            return View(model);
        }

        [HttpPost(PagePaths.PermitForRecycleWaste)]
        public async Task<IActionResult> SelectAuthorisationType(SelectAuthorisationTypeViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.WastePermitExemptions, PagePaths.PermitForRecycleWaste];

            SetBackLink(session, PagePaths.PermitForRecycleWaste);

            var selectedText = model.AuthorisationTypes.Find(x => x.Id == model.SelectedAuthorisation)?.SelectedAuthorisationText;
            var hasData = !string.IsNullOrEmpty(selectedText);
            string message;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parsedSelectedAuthorisation = (MaterialPermitType)model.SelectedAuthorisation!;
            var errorKeyForPermitNumberInput = $"{parsedSelectedAuthorisation}_number_input";
            switch ((MaterialPermitType)model.SelectedAuthorisation!)
            {
                case MaterialPermitType.EnvironmentalPermitOrWasteManagementLicence when !hasData:
                    message = Resources.Views.Registration.SelectAuthorisationType.error_message_enter_permit_or_license_number;
                    ModelState.AddModelError(errorKeyForPermitNumberInput, message);
                    break;
                case MaterialPermitType.WasteManagementLicence or MaterialPermitType.PollutionPreventionAndControlPermit or MaterialPermitType.InstallationPermit when !hasData:
                    message = Resources.Views.Registration.SelectAuthorisationType.enter_permit_or_license_number;
                    ModelState.AddModelError(errorKeyForPermitNumberInput, message);
                    break;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            session.RegistrationApplicationSession.RegistrationTasks.SetTaskAsInProgress(TaskType.WasteLicensesPermitsExemptions);
            session.RegistrationApplicationSession.WasteDetails!.SetSelectedAuthorisation((PermitType?)model.SelectedAuthorisation, selectedText);

            await SaveSession(session, PagePaths.PermitForRecycleWaste);

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails.RegistrationMaterialId.HasValue)
            {
                var dto = new UpdateRegistrationMaterialPermitsDto
                {
                    PermitNumber = selectedText,
                    PermitTypeId = model.SelectedAuthorisation,
                };

                await ReprocessorService
                    .RegistrationMaterials
                    .UpdateRegistrationMaterialPermitsAsync(wasteDetails.RegistrationMaterialId.Value, dto);
            }

            if (buttonAction == SaveAndContinueActionKey)
            {
                var option = (MaterialPermitType)model.SelectedAuthorisation;
                var redirectMap = new Dictionary<MaterialPermitType, string>
                {
                    { MaterialPermitType.EnvironmentalPermitOrWasteManagementLicence, PagePaths.EnvironmentalPermitOrWasteManagementLicence },
                    { MaterialPermitType.InstallationPermit, PagePaths.InstallationPermit },
                    { MaterialPermitType.PollutionPreventionAndControlPermit, PagePaths.PpcPermit },
                    { MaterialPermitType.WasteManagementLicence, PagePaths.WasteManagementLicense },
                    { MaterialPermitType.WasteExemption, PagePaths.ExemptionReferences }
                };

                if (redirectMap.TryGetValue(option, out var redirectPath))
                {
                    return Redirect(redirectPath);
                }
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);
        }

        [HttpGet(PagePaths.WasteManagementLicense)]
        public async Task<IActionResult> ProvideWasteManagementLicense()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense];

            SetBackLink(session, PagePaths.WasteManagementLicense);

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            if (wasteDetails?.CurrentMaterialApplyingFor is null)
            {
                return RedirectToAction(nameof(WastePermitExemptions));
            }

            var model = new MaterialPermitViewModel
            {
                MaterialType = MaterialType.Licence,
                Material = wasteDetails.CurrentMaterialApplyingFor.Name.GetMaterialName()
            };

            if (wasteDetails.CurrentMaterialApplyingFor.PermitType is PermitType.WasteManagementLicence)
            {
                var currentMaterial = wasteDetails.CurrentMaterialApplyingFor;

                model.MaximumWeight = currentMaterial.WeightInTonnes.ToString(CultureInfo.InvariantCulture);
                model.SelectedFrequency = (PermitPeriod?)(int?)currentMaterial.PermitPeriod;
            }

            return View(model);
        }

        [HttpPost(PagePaths.WasteManagementLicense)]
        public async Task<IActionResult> ProvideWasteManagementLicense(MaterialPermitViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense];

            SetBackLink(session, PagePaths.WasteManagementLicense);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var wasteDetails = session.RegistrationApplicationSession.WasteDetails;

            var capacityInTonnes = decimal.Parse(model.MaximumWeight!);
            var selectedFrequency = (int)model.SelectedFrequency!;

            wasteDetails!.SetWasteManagementLicence(capacityInTonnes, selectedFrequency);
            await SaveSession(session, PagePaths.WasteManagementLicense);

            if (wasteDetails.RegistrationMaterialId.HasValue)
            {
                var dto = new UpdateRegistrationMaterialPermitCapacityDto
                {
                    PermitTypeId = (int)wasteDetails.CurrentMaterialApplyingFor.PermitType,
                    CapacityInTonnes = capacityInTonnes,
                    PeriodId = selectedFrequency,
                };

                await ReprocessorService
                    .RegistrationMaterials
                    .UpdateRegistrationMaterialPermitCapacityAsync(wasteDetails.RegistrationMaterialId.Value, dto);
            }

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.MaximumWeightSiteCanReprocess);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(model);

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

            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();

            var currentMaterial = session.RegistrationApplicationSession.WasteDetails.CurrentMaterialApplyingFor;

            var exemptions = new List<Exemption>();

            if (!string.IsNullOrEmpty(viewModel.ExemptionReferences1))
                exemptions.Add(new Exemption { ReferenceNumber = viewModel.ExemptionReferences1 });

            if (!string.IsNullOrEmpty(viewModel.ExemptionReferences2))
                exemptions.Add(new Exemption { ReferenceNumber = viewModel.ExemptionReferences2 });

            if (!string.IsNullOrEmpty(viewModel.ExemptionReferences3))
                exemptions.Add(new Exemption { ReferenceNumber = viewModel.ExemptionReferences3 });

            if (!string.IsNullOrEmpty(viewModel.ExemptionReferences4))
                exemptions.Add(new Exemption { ReferenceNumber = viewModel.ExemptionReferences4 });

            if (!string.IsNullOrEmpty(viewModel.ExemptionReferences5))
                exemptions.Add(new Exemption { ReferenceNumber = viewModel.ExemptionReferences5 });


            if (currentMaterial is null)
            {
                return Redirect(PagePaths.WastePermitExemptions);
            }

            currentMaterial.SetExemptions(exemptions);

            await SaveSession(session, PagePaths.ExemptionReferences);

            var registrationId = session.RegistrationId!.Value;

            Guid registrationMaterialId = session.RegistrationApplicationSession.WasteDetails.CurrentMaterialApplyingFor.Id;

            //TODO: Remove this when the registrationMaterialId is set correctly in the session.
            if (registrationMaterialId == Guid.Empty)
            {
                var materialRegistrations = await ReprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsAsync(registrationId);

                if (materialRegistrations.Count > 0)
                {
                    registrationMaterialId = materialRegistrations[0].Id;
                }
            }

            var exemptionDtos = exemptions
                .Where(e => !string.IsNullOrEmpty(e.ReferenceNumber))
                .Select(e => new MaterialExemptionReferenceDto
                {
                    ReferenceNumber = e.ReferenceNumber
                })
                .ToList();

            var exemptionReferencesDto = new CreateExemptionReferencesDto
            {
                RegistrationMaterialId = registrationMaterialId,
                MaterialExemptionReferences = exemptionDtos
            };

            await ReprocessorService.RegistrationMaterials.CreateExemptionReferences(exemptionReferencesDto);

            if (buttonAction == SaveAndContinueActionKey)
            {
                return Redirect(PagePaths.MaximumWeightSiteCanReprocess);
            }

            if (buttonAction == SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return View(nameof(ExemptionReferences), viewModel);
        }       


        #region private methods
        [ExcludeFromCodeCoverage]
        private async Task MarkTaskStatusAsCompleted(TaskType taskType)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session);

            if (session?.RegistrationId is not null)
            {
                var registrationId = session.RegistrationId.Value;
                var updateRegistrationTaskStatusDto = new UpdateRegistrationTaskStatusDto
                {
                    TaskName = taskType.ToString(),
                    Status = nameof(TaskStatuses.Completed),
                };

                try
                {
                    await ReprocessorService.Registrations.UpdateRegistrationTaskStatusAsync(registrationId, updateRegistrationTaskStatusDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to call facade for UpdateRegistrationTaskStatusAsync");
                    throw;
                }
            }
        }

        #endregion
    }
}