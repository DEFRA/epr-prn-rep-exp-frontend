using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Exporter;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IMapper mapper,
    IRegistrationService registrationService,
    IValidationService validationService,
    IReprocessorService reprocessorService,
    IExporterRegistrationService exporter) : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
    protected const string NoOverseasReprocessorSiteError = "You must have at least one overseas reprocessors site before you can continue";

    [HttpGet]
    [Route(PagePaths.OverseasSiteDetails)]
    public async Task<IActionResult> OverseasSiteDetails()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession?.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.ExporterTaskList, PagePaths.OverseasSiteDetails];

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites ??= new OverseasReprocessingSites();
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses;
        overseasAddresses ??= new List<OverseasAddress>();
        var activeOverseasAddress = overseasAddresses.SingleOrDefault(oa => oa.IsActive);

        OverseasReprocessorSiteViewModel model;
        if (activeOverseasAddress != null)
        {
            model = mapper.Map<OverseasReprocessorSiteViewModel>(activeOverseasAddress);
        }
        else
        {
            model = new OverseasReprocessorSiteViewModel();
        }

        model.Countries = await registrationService.GetCountries();

        SetBackLink(session, PagePaths.OverseasSiteDetails);
        await SaveSession(session, PagePaths.OverseasSiteDetails);
        return View("~/Views/Registration/Exporter/OverseasSiteDetails.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.OverseasSiteDetails)]
    public async Task<IActionResult> OverseasSiteDetails(OverseasReprocessorSiteViewModel model, string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.ExporterTaskList, PagePaths.OverseasSiteDetails];
        SetBackLink(session, PagePaths.OverseasSiteDetails);

        var validationResult = await validationService.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            model.Countries = await registrationService.GetCountries();
            return View("~/Views/Registration/Exporter/OverseasSiteDetails.cshtml", model);
        }

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites ??= new OverseasReprocessingSites();

        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses;
        overseasAddresses ??= new List<OverseasAddress>();
        var activeOverseasAddress = overseasAddresses.SingleOrDefault(oa => oa.IsActive);

        if (activeOverseasAddress != null)
        {
            mapper.Map(model, activeOverseasAddress);
        }
        else
        {
            var overseasAddress = mapper.Map<OverseasAddress>(model);
            overseasAddress.IsActive = true;

            overseasAddresses.Add(overseasAddress);
        }


        await SaveSession(session, PagePaths.OverseasSiteDetails);
        return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.BaselConventionAndOECDCodes, PagePaths.ApplicationSaved);
    }

    [HttpGet]
    [Route(PagePaths.BaselConventionAndOECDCodes)]
    public async Task<IActionResult> BaselConventionAndOECDCodes()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.RegistrationLanding, PagePaths.OverseasSiteDetails, PagePaths.BaselConventionAndOECDCodes];

        var overseasAddressActiveRecord = session.ExporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses?.Find(x => x.IsActive);

        if (overseasAddressActiveRecord is null)
        {
            throw new InvalidOperationException(nameof(overseasAddressActiveRecord));
        }

        var model = new BaselConventionAndOecdCodesViewModel
        {
            MaterialName = session.ExporterRegistrationApplicationSession!.MaterialName,
            OrganisationName = overseasAddressActiveRecord.OrganisationName!,
            AddressLine1 = overseasAddressActiveRecord.AddressLine1
        };

        model.OecdCodes = overseasAddressActiveRecord.OverseasAddressWasteCodes
            .Select(code => new OverseasAddressWasteCodesViewModel() { CodeName = code.CodeName })
            .ToList();

        while (model.OecdCodes.Count < 5)
        {
            model.OecdCodes.Add(new OverseasAddressWasteCodesViewModel());
        }

        SetBackLink(session, PagePaths.BaselConventionAndOECDCodes);
        await SaveSession(session, PagePaths.BaselConventionAndOECDCodes);

        ViewData.ModelState.Clear();
        return View("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.BaselConventionAndOECDCodes)]
    public async Task<IActionResult> BaselConventionAndOECDCodes(BaselConventionAndOecdCodesViewModel model, string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.RegistrationLanding, PagePaths.OverseasSiteDetails, PagePaths.BaselConventionAndOECDCodes];

        SetBackLink(session, PagePaths.BaselConventionAndOECDCodes);

        var validationResult = await validationService.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml", model);
        }

        var exporterRegistrationApplicationSession = session.ExporterRegistrationApplicationSession;

        //todo -- remove this as part of integration as we expect exporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses to exist.
        if (exporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses == null)
        {
            exporterRegistrationApplicationSession!.OverseasReprocessingSites!.OverseasAddresses = new();
        }

        var activeRecord = exporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Find(x => x.IsActive);
        if (activeRecord != null)
        {
            activeRecord.OverseasAddressWasteCodes = model.OecdCodes.Where(c => !string.IsNullOrWhiteSpace(c.CodeName))
                                                                    .Select(c => new OverseasAddressWasteCodes { CodeName = c.CodeName!.Trim() })
                                                                    .ToList();
        }

        SetBackLink(session, PagePaths.BaselConventionAndOECDCodes);
        await SaveSession(session, PagePaths.BaselConventionAndOECDCodes);

        return buttonAction switch
        {
            SaveAndContinueActionKey => Redirect(PagePaths.AddAnotherOverseasReprocessingSite),
            SaveAndComeBackLaterActionKey => Redirect(PagePaths.ApplicationSaved),
            _ => View("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml", model)
        };
    }

    [HttpGet]
    [Route(PagePaths.ExporterInterimSiteQuestionOne)]
    public async Task<IActionResult> InterimSitesQuestionOne()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        if (session?.RegistrationId is null)
            return Redirect("/Error");

        session.Journey = [PagePaths.RegistrationLanding, PagePaths.ExporterRegistrationTaskList, PagePaths.ExporterInterimSiteQuestionOne];

        SetBackLink(session, PagePaths.ExporterInterimSiteQuestionOne);
        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);

        return View("~/Views/Registration/Exporter/InterimSitesQuestionOne.cshtml", new InterimSitesQuestionOneViewModel());
    }

    [HttpPost]
    [Route(PagePaths.ExporterInterimSiteQuestionOne)]
    public async Task<IActionResult> InterimSitesQuestionOne(InterimSitesQuestionOneViewModel model, string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }
        session.Journey = [PagePaths.RegistrationLanding, PagePaths.ExporterRegistrationTaskList, PagePaths.ExporterInterimSiteQuestionOne];

        SetBackLink(session, PagePaths.ExporterInterimSiteQuestionOne);

        var validationResult = await validationService.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View("~/Views/Registration/Exporter/InterimSitesQuestionOne.cshtml", model);
        }

        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);

        if (buttonAction == SaveAndContinueActionKey)
        {
            if (model.HasInterimSites == true)
                return Redirect(PagePaths.ExporterAddInterimSites);

            await MarkTaskStatusAsCompleted(TaskType.InterimSites);
            return Redirect(PagePaths.ExporterRegistrationTaskList);
        }

        return Redirect(PagePaths.ApplicationSaved);
    }

    private async Task MarkTaskStatusAsCompleted(TaskType taskType)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is not null)
        {
            var registrationMaterialId = session.ExporterRegistrationApplicationSession.RegistrationMaterialId.Value;
            var updateRegistrationTaskStatusDto = new UpdateRegistrationTaskStatusDto
            {
                TaskName = taskType.ToString(),
                Status = nameof(TaskStatuses.Completed),
            };

            await reprocessorService.RegistrationMaterials.UpdateApplicationRegistrationTaskStatusAsync(registrationMaterialId, updateRegistrationTaskStatusDto); 
        }
    }

    [HttpGet]
    [Route(PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task<IActionResult> CheckOverseasReprocessingSitesAnswers([FromQuery] string? buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite];
        SetBackLink(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        if (session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count == 0 && !string.IsNullOrEmpty(buttonAction))
        {
            var modelError = new CheckOverseasReprocessingSitesAnswersViewModel(session.ExporterRegistrationApplicationSession);

            ModelState.AddModelError(nameof(modelError.OverseasAddresses), NoOverseasReprocessorSiteError);
            return View("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml", modelError);
        }

        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.ExporterRegistrationApplicationSession);

        return View("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task<IActionResult> CheckOverseasReprocessingSitesAnswers(CheckOverseasReprocessingSitesAnswersViewModel model, string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite];

        SetBackLink(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);
            return ReturnSaveAndContinueRedirect(buttonAction, string.Empty, PagePaths.ApplicationSaved);
        }

        if (session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count == 0
            && buttonAction == SaveAndContinueActionKey)
        {
            return RedirectToAction(nameof(CheckOverseasReprocessingSitesAnswers), new { buttonAction = buttonAction });
        }

        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        await exporter.SaveOverseasReprocessorAsync(mapper.Map<OverseasAddressRequestDto>(session.ExporterRegistrationApplicationSession));
        return Redirect(PagePaths.ExporterTaskList);
    }

    [HttpGet]
    [Route(PagePaths.ChangeOverseasReprocessingSite)]
    public async Task<IActionResult> ChangeOverseasReprocessingSite([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        for (int i = 0; i < overseasAddresses.Count; i++)
        {
            overseasAddresses[i].IsActive = (i == index - 1);
        }

        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        return RedirectToAction(nameof(OverseasSiteDetails));
    }

    [HttpGet]
    [Route(PagePaths.DeleteOverseasReprocessingSite)]
    public async Task<IActionResult> DeleteOverseasReprocessingSite([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite];

        var overseasAddress = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();
        var siteToDelete = overseasAddress[index - 1];
        TempData["DeletedOverseasReprocessor"] = $"{siteToDelete.OrganisationName}, {siteToDelete.AddressLine1}";

        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.ExporterRegistrationApplicationSession);
        model.OverseasAddresses.Remove(siteToDelete);
        SetBackLink(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);
        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        return RedirectToAction(nameof(CheckOverseasReprocessingSitesAnswers));
    }

    [HttpGet]
    [Route(PagePaths.ChangeBaselConvention)]
    public async Task<IActionResult> ChangeBaselConvention([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        for (int i = 0; i < overseasAddresses.Count; i++)
        {
            overseasAddresses[i].IsActive = (i == index - 1);
        }

        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        return RedirectToAction(nameof(BaselConventionAndOECDCodes));
    }

    [HttpGet]
    [Route(PagePaths.AddAnotherOverseasReprocessingSiteFromCheckYourAnswer)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSiteFromCheckYourAnswer()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        for (int i = 0; i < overseasAddresses.Count; i++)
        {
            overseasAddresses[i].IsActive = false;
        }

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);

        return RedirectToAction(nameof(OverseasSiteDetails));
    }

    [HttpGet]
    [Route(PagePaths.ExporterInterimSiteDetails)]
    public async Task<IActionResult> InterimSiteDetails()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession?.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        var activeOverseasAddress = session.ExporterRegistrationApplicationSession.InterimSites?.OverseasMaterialReprocessingSites?.SingleOrDefault(isa => isa.IsActive);

        if (activeOverseasAddress is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.ExporterAddInterimSites, PagePaths.ExporterInterimSiteDetails];

        var activeInterimSiteAddress = activeOverseasAddress.InterimSiteAddresses.SingleOrDefault(isa => isa.IsActive);

        InterimSiteViewModel model;
        if (activeInterimSiteAddress != null)
        {
            model = mapper.Map<InterimSiteViewModel>(activeInterimSiteAddress);
        }
        else
        {
            model = new InterimSiteViewModel();
        }

        model.OverseasSiteOrganisationName = activeOverseasAddress.OverseasAddress.OrganisationName;
        model.OverseasSiteAddressLine1 = activeOverseasAddress.OverseasAddress.AddressLine1;
        model.Countries = await registrationService.GetCountries();

        SetBackLink(session, PagePaths.ExporterInterimSiteDetails);
        await SaveSession(session, PagePaths.ExporterInterimSiteDetails);
        return View("~/Views/Registration/Exporter/InterimSiteDetails.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.ExporterInterimSiteDetails)]
    public async Task<IActionResult> InterimSiteDetails(InterimSiteViewModel model, string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession?.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.ExporterAddInterimSites, PagePaths.ExporterInterimSiteDetails];
        SetBackLink(session, PagePaths.ExporterInterimSiteDetails);

        var validationResult = await validationService.ValidateAsync(model);

        var activeOverseasAddress = session.ExporterRegistrationApplicationSession.InterimSites?.OverseasMaterialReprocessingSites?.SingleOrDefault(isa => isa.IsActive);
        if (activeOverseasAddress is null)
        {
            return Redirect("/Error");
        }

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);

            model.OverseasSiteOrganisationName = activeOverseasAddress.OverseasAddress.OrganisationName;
            model.OverseasSiteAddressLine1 = activeOverseasAddress.OverseasAddress.AddressLine1;
            model.Countries = await registrationService.GetCountries();
            return View("~/Views/Registration/Exporter/InterimSiteDetails.cshtml", model);
        }

        var activeInterimSiteAddress = activeOverseasAddress.InterimSiteAddresses.SingleOrDefault(isa => isa.IsActive);
        if (activeInterimSiteAddress != null)
        {
            mapper.Map(model, activeInterimSiteAddress);
        }
        else
        {
            var newInterimSiteAddress = mapper.Map<InterimSiteAddress>(model);
            newInterimSiteAddress.IsActive = true;

            activeOverseasAddress.InterimSiteAddresses.Add(newInterimSiteAddress);
        }

        await SaveSession(session, PagePaths.ExporterInterimSiteDetails);
        return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.ExporterAnotherInterimSite, PagePaths.ApplicationSaved);
    }

    [HttpGet]
    [Route(PagePaths.ExporterInterimSitesUsed)]
    public async Task<IActionResult> InterimSiteUsed([FromQuery] string? buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession?.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = [PagePaths.ExporterAnotherInterimSite, PagePaths.ExporterInterimSitesUsed];

        var activeOverseasSite = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Find(x => x.IsActive);

        var model = new CheckInterimSitesAnswersViewModel(activeOverseasSite);

        SetBackLink(session, PagePaths.ExporterInterimSitesUsed);
        await SaveSession(session, PagePaths.ExporterInterimSitesUsed);
        return View("~/Views/Registration/Exporter/CheckInterimSitesAnswers.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.ExporterInterimSitesUsed)]
    public async Task<IActionResult> ExporterInterimSitesUsed(string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.ExporterAnotherInterimSite, PagePaths.ExporterInterimSitesUsed];

        SetBackLink(session, PagePaths.ExporterInterimSitesUsed);

        await SaveSession(session, PagePaths.ExporterInterimSitesUsed);
        return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.ExporterAddInterimSites, PagePaths.ApplicationSaved);
    }

    [HttpGet]
    [Route(PagePaths.ChangeInterimSiteDetails)]
    public async Task<IActionResult> ChangeInterimSiteDetails([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var activeOverseasSite = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Find(x => x.IsActive);

        var interimAddresses = activeOverseasSite.InterimSiteAddresses.OrderBy(a => a.OrganisationName).ToList();

        for (int i = 0; i < interimAddresses.Count; i++)
        {
            interimAddresses[i].IsActive = (i == index - 1);
        }

        await SaveSession(session, PagePaths.ExporterInterimSitesUsed);

        return RedirectToAction(nameof(InterimSiteDetails));
    }

    [HttpGet]
    [Route(PagePaths.DeleteInterimSite)]
    public async Task<IActionResult> DeleteInterimSite([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.ExporterAnotherInterimSite, PagePaths.ExporterInterimSitesUsed];

        var activeOverseasSite = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Find(x => x.IsActive);

        var interimAddresses = activeOverseasSite.InterimSiteAddresses.OrderBy(a => a.OrganisationName).ToList();
        var siteToDelete = interimAddresses[index - 1];
        TempData["DeletedInterimSite"] = $"{siteToDelete.OrganisationName}, {siteToDelete.AddressLine1}";

        var model = new CheckInterimSitesAnswersViewModel(activeOverseasSite);
        model.InterimSiteAddresses.Remove(siteToDelete);
        SetBackLink(session, PagePaths.ExporterInterimSitesUsed);
        await SaveSession(session, PagePaths.ExporterInterimSitesUsed);

        return RedirectToAction(nameof(InterimSiteUsed));
    }

    [HttpGet]
    [Route(PagePaths.AddAnotherInterimSiteFromCheckYourAnswer)]
    public async Task<IActionResult> AddAnotherInterimSiteFromCheckYourAnswer()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var activeOverseasSite = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Find(x => x.IsActive);

        var interimAddresses = activeOverseasSite.InterimSiteAddresses.ToList();

        for (int i = 0; i < interimAddresses.Count; i++)
        {
            interimAddresses[i].IsActive = false;
        }

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);

        return RedirectToAction(nameof(AddInterimSites));
    }

    [HttpGet]
    [Route(PagePaths.ExporterAddInterimSites)]
    public async Task<IActionResult> AddInterimSites()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.RegistrationLanding, PagePaths.ExporterAddInterimSites];
        var exporterRegistrationApplicationSession = session.ExporterRegistrationApplicationSession;
        exporterRegistrationApplicationSession.InterimSites ??= new InterimSites { OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>() };

        var overseasMaterialReprocessingSitesByRegistrationMaterialSavedData = await exporter.GetOverseasMaterialReprocessingSites((Guid)session.ExporterRegistrationApplicationSession.RegistrationMaterialId!);
        ReconcileSessionData(exporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites, overseasMaterialReprocessingSitesByRegistrationMaterialSavedData);

        var viewModel = new AddInterimSitesViewModel()
        {
            OverseasMaterialReprocessingSites = exporterRegistrationApplicationSession!
                .InterimSites!
                .OverseasMaterialReprocessingSites
                .OrderBy(site => site.OverseasAddress.OrganisationName)
                .ToList()
        };

        SetBackLink(session, PagePaths.ExporterAddInterimSites);
        await SaveSession(session, PagePaths.ExporterAddInterimSites);
        return View("~/Views/Registration/Exporter/AddInterimSites.cshtml", viewModel);
    }

    [HttpPost]
    [Route(PagePaths.ExporterAddInterimSites)]
    public async Task<IActionResult> AddInterimSites(string buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        SetBackLink(session, PagePaths.ExporterAddInterimSites);
        await SaveSession(session, PagePaths.ExporterAddInterimSites);

        switch (buttonAction)
        {
            case SaveAndComeBackLaterActionKey:
                return Redirect(PagePaths.ApplicationSaved);
            case SaveAndContinueActionKey:
                {
                    var exporterRegistrationApplicationSession = session.ExporterRegistrationApplicationSession;
                    exporterRegistrationApplicationSession.InterimSites ??= new InterimSites();

                    await exporter.UpsertInterimSitesAsync(mapper.Map<SaveInterimSitesRequestDto>(session.ExporterRegistrationApplicationSession));

                    //reset session data from database 
                    var overseasMaterialReprocessingSitesByRegistrationMaterialSavedData = await exporter.GetOverseasMaterialReprocessingSites((Guid)session.ExporterRegistrationApplicationSession.RegistrationMaterialId!);
                    exporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>();
                    ReconcileSessionData(exporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites, overseasMaterialReprocessingSitesByRegistrationMaterialSavedData);

                    Redirect(PagePaths.RegistrationLanding);
                    break;
                }
        }

        return RedirectToAction(nameof(AddInterimSites));
    }

    protected void ReconcileSessionData(List<OverseasMaterialReprocessingSite> OverseasMaterialReprocessingSitesSessionData, List<OverseasMaterialReprocessingSiteDto>? OverseasMaterialReprocessingSitesSavedData)
    {
        if (OverseasMaterialReprocessingSitesSavedData is null)
        {
            OverseasMaterialReprocessingSitesSessionData.Clear();
            return;
        }
        var savedIds = OverseasMaterialReprocessingSitesSavedData.Select(x => x.OverseasAddressId).ToHashSet();
        OverseasMaterialReprocessingSitesSessionData.RemoveAll(site => !savedIds.Contains(site.OverseasAddressId));

        var sessionIds = OverseasMaterialReprocessingSitesSessionData.Select(x => x.OverseasAddressId).ToHashSet();
        var newSites = OverseasMaterialReprocessingSitesSavedData.Where(dto => !sessionIds.Contains(dto.OverseasAddressId)).ToList();

        foreach (var dto in newSites)
        {
            var mapped = mapper.Map<OverseasMaterialReprocessingSite>(dto);
            mapped.IsActive = false;
            OverseasMaterialReprocessingSitesSessionData.Add(mapped);
        }
    }

    [Route("CheckAddedInterimSites")]
    public async Task<IActionResult> CheckAddedInterimSites(Guid overseasAddressId)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var sites = session?.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites;

        if (sites == null)
        {
            return NotFound();
        }

        foreach (var site in sites)
        {
            site.IsActive = site.OverseasAddressId == overseasAddressId;
        }

        await SaveSession(session, PagePaths.ExporterAddInterimSites);

        var selectedSite = sites.Find(s => s.OverseasAddressId == overseasAddressId);
        if (selectedSite == null)
        {
            return NotFound();
        }

        return Redirect(PagePaths.ExporterInterimSitesUsed);
    }

    [Route("AddNewInterimSite")]
    public async Task<IActionResult> AddNewInterimSite(Guid overseasAddressId)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var sites = session?.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites;

        if (sites == null)
        {
            return NotFound();
        }

        foreach (var site in sites)
        {
            site.IsActive = site.OverseasAddressId == overseasAddressId;
        }

        await SaveSession(session, PagePaths.ExporterAddInterimSites);

        var selectedSite = sites.Find(s => s.OverseasAddressId == overseasAddressId);
        if (selectedSite == null)
        {
            return NotFound();
        }

        return Redirect(PagePaths.ExporterInterimSiteDetails);
    }

    /// <summary>
    /// Save the current session.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    /// <returns>The completed task.</returns>
    protected async Task SaveSession(ExporterRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await sessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Clears the journey in the session from the current page onwards, effectively resetting the journey for the user.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="currentPagePath"></param>
    protected static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
    {
        var index = session.Journey.IndexOf(currentPagePath);

        // this also cover if current page not found (index = -1) then it clears all pages
        session.Journey = session.Journey.Take(index + 1).ToList();
    }

    /// <summary>
    /// Temporary method to set the back link.
    /// </summary>
    /// <remarks>Differs to the SetBackLink method by explicitly setting the previous page rather than using PreviousOrDefault.</remarks>
    /// <param name="previousPagePath">The path to the previous page.</param>
    /// <param name="currentPagePath">The path to the current page.</param>
    /// <returns>The completed page.</returns>
    protected async Task SetTempBackLink(string previousPagePath, string currentPagePath)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();
        session.Journey = [previousPagePath, currentPagePath];
        SetBackLink(session, currentPagePath);

        await SaveSession(session, previousPagePath);
    }

    /// <summary>
    /// Sets the back link for the current page in the session's journey.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    protected void SetBackLink(ExporterRegistrationSession session, string currentPagePath)
    {
        ViewBag.BackLinkToDisplay = session.Journey!.PreviousOrDefault(currentPagePath) ?? string.Empty;
    }

    /// <summary>
    /// Handles the save and continue handlers.
    /// </summary>
    /// <param name="buttonAction">The name of the handler i.e. SaveAndContinue or SaveAndComeBackLater.</param>
    /// <param name="saveAndContinueRedirectUrl">The url to redirect to for the save and continue handler.</param>
    /// <param name="saveAndComeBackLaterRedirectUrl">The url to redirect to for the save and come back later handler.</param>
    /// <returns>A redirect result.</returns>
    protected RedirectResult ReturnSaveAndContinueRedirect(string buttonAction, string saveAndContinueRedirectUrl,
        string saveAndComeBackLaterRedirectUrl)
    {
        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(saveAndContinueRedirectUrl);
        }

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(saveAndComeBackLaterRedirectUrl);
        }
        return Redirect("/Error");
    }


    [HttpGet]
    [Route(PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSite()
    {
        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.AddAnotherOverseasReprocessingSite);

        return View(nameof(AddAnotherOverseasReprocessingSite));
    }

    [HttpPost]
    [Route(PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSite(AddAnotherOverseasReprocessingSiteViewModel model, string buttonAction)
    {
        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.AddAnotherOverseasReprocessingSite);

        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }


        var validationResult = await validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        overseasAddresses.ForEach(a => a.IsActive = false);

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);

        return await RedirectToCorrectPage(model, buttonAction);
    }


    [HttpGet]
    [Route(PagePaths.ExporterAnotherInterimSite)]
    public async Task<IActionResult> UseAnotherInterimSite()
    {
        await SetTempBackLink(PagePaths.ExporterInterimSiteDetails, PagePaths.ExporterAnotherInterimSite);

        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        UseAnotherInterimSiteViewModel model = new UseAnotherInterimSiteViewModel();

        var activeOverseasAddress = session.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites?.SingleOrDefault(o => o.IsActive);

        if (activeOverseasAddress is null)
        {
            return Redirect("/Error");
        }

        model.CompanyName = activeOverseasAddress!.OverseasAddress!.OrganisationName;
        model.AddressLine = activeOverseasAddress!.OverseasAddress!.AddressLine1;        

        return View(nameof(UseAnotherInterimSite), model);
    }

    [HttpPost]
    [Route(PagePaths.ExporterAnotherInterimSite)]
    public async Task<IActionResult> UseAnotherInterimSite(UseAnotherInterimSiteViewModel model, string buttonAction)
    {
        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.ExporterAnotherInterimSite);

        var accepted = model.AddInterimSiteAccepted.GetValueOrDefault();

        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        var validationResult = await validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var overseasAddresses = session.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites?.OrderBy(a => a.OverseasAddress.OrganisationName).ToList();

        if (accepted && buttonAction == SaveAndContinueActionKey)
        {
            overseasAddresses?.ForEach(o => o.InterimSiteAddresses?.ForEach(a => a.IsActive = false));
        }        

        await SaveSession(session, PagePaths.ExporterAnotherInterimSite);        

        return (accepted, buttonAction) switch
        {
            (true, SaveAndContinueActionKey) => Redirect(PagePaths.ExporterInterimSiteDetails),
            (false, SaveAndContinueActionKey) => Redirect(PagePaths.ExporterInterimSitesUsed),
            (true, SaveAndComeBackLaterActionKey) => Redirect(PagePaths.ApplicationSaved),
            (false, SaveAndComeBackLaterActionKey) => Redirect(PagePaths.ApplicationSaved),
            _ => View(model)
        };        
    }

    
    private async Task<IActionResult> RedirectToCorrectPage(AddAnotherOverseasReprocessingSiteViewModel model, string buttonAction)
    {
        var accepted = model.AddOverseasSiteAccepted.GetValueOrDefault();

        return (accepted, buttonAction) switch
        {
            (true, SaveAndContinueActionKey) => Redirect(PagePaths.OverseasSiteDetails),
            (true, SaveAndComeBackLaterActionKey) => Redirect(PagePaths.ApplicationSaved),
            (false, SaveAndContinueActionKey) => Redirect(PagePaths.CheckYourAnswersForOverseasProcessingSite),
            (false, SaveAndComeBackLaterActionKey) => Redirect(PagePaths.ApplicationSaved),
            _ => View(model)
        };        
    }
}