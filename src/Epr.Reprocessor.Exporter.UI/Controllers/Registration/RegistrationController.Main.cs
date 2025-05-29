using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    public partial class RegistrationController
    {
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
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.MaximumWeightSiteCanReprocess };

            SetBackLink(session, PagePaths.MaximumWeightSiteCanReprocess);

            if (!ModelState.IsValid)
            {
                return View(nameof(MaximumWeightSiteCanReprocess), viewModel);
            }

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
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.EnvironmentalPermitOrWasteManagementLicence };

            SetBackLink(session, PagePaths.EnvironmentalPermitOrWasteManagementLicence);

            if (!ModelState.IsValid)
            {
                return View(nameof(EnvironmentalPermitOrWasteManagementLicence), viewModel);
            }

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
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.InstallationPermit };

            SetBackLink(session, PagePaths.InstallationPermit);

            if (!ModelState.IsValid)
            {
                return View(nameof(InstallationPermit), viewModel);
            }

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
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.PpcPermit };

            SetBackLink(session, PagePaths.PpcPermit);

            if (!ModelState.IsValid)
            {
                return View(nameof(PpcPermit), viewModel);
            }

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
            SetTempBackLink(PagePaths.AddressForNotices, PagePaths.WastePermitExemptions);

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
            session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.GridReferenceForEnteredReprocessingSite };

            session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.GridReferenceForEnteredReprocessingSite);
            await SaveSession(session, PagePaths.GridReferenceForEnteredReprocessingSite, PagePaths.RegistrationLanding);

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

            // Mark task status as completed
            await MarkTaskStatusAsCompleted(RegistrationTaskType.SiteAddressAndContactDetails);

            return Redirect(PagePaths.RegistrationLanding);
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
    }
}