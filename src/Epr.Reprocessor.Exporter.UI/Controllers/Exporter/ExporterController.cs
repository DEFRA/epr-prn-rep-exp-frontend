using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;


[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IMapper mapper,
    IRegistrationService registrationService,
    IValidationService validationService,
    IReprocessorService reprocessorService,
    ILogger<ExporterController> logger) : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

    [HttpGet]
    [Route(PagePaths.OverseasSiteDetails)]
    public async Task<IActionResult> Index()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession?.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = ["test-setup-session", PagePaths.OverseasSiteDetails];

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites ??= new OverseasReprocessingSites();
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses;
        overseasAddresses ??= new List<OverseasAddress>();
        var activeOverseasAddress = overseasAddresses.SingleOrDefault(oa => oa.IsActive);

        OverseasReprocessorSiteViewModel model;
        if (activeOverseasAddress != null)
        {
            model = mapper.Map<OverseasReprocessorSiteViewModel>(activeOverseasAddress);
            model.IsFirstSite = overseasAddresses.Count == 1;
        }
        else
        { 
            model = new OverseasReprocessorSiteViewModel();
            model.IsFirstSite = !(overseasAddresses.Count == 0);
        }

        model.Countries = await registrationService.GetCountries();

        SetBackLink(session, PagePaths.OverseasSiteDetails);
        await SaveSession(session, PagePaths.OverseasSiteDetails);
        return View("~/Views/Registration/Exporter/OverseasSiteDetails.cshtml", model);
    }

    [HttpPost]
    [Route(PagePaths.OverseasSiteDetails)]
    public async Task<IActionResult> Index(OverseasReprocessorSiteViewModel model, string buttonAction)
    {
        if (!ModelState.IsValid)
        {
            model.Countries = await registrationService.GetCountries();
            return View("~/Views/Registration/Exporter/OverseasSiteDetails.cshtml", model);
        }

        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }
        session.Journey = ["test-setup-session", PagePaths.OverseasSiteDetails];

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites ??= new OverseasReprocessingSites();

        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses;
        overseasAddresses ??= new List<OverseasAddress>();
        var activeOverseasAddress = overseasAddresses.SingleOrDefault(oa => oa.IsActive);

        if (activeOverseasAddress != null)
        {
            mapper.Map(model, activeOverseasAddress);
            model.IsFirstSite = overseasAddresses.Count == 1;
        }
        else
        {
            var overseasAddress = mapper.Map<OverseasAddress>(model);
            overseasAddress.IsActive = true;
            model.IsFirstSite = !(overseasAddresses.Count == 0);

            overseasAddresses.Add(overseasAddress);
        }

        SetBackLink(session, PagePaths.OverseasSiteDetails);
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
        {
            return Redirect("/Error");
        }

        SetBackLink(session, PagePaths.ExporterTaskList);
        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);

        return View(nameof(InterimSitesQuestionOne), new InterimSitesQuestionOneViewModel());
    }

    [HttpPost]
    [Route(PagePaths.ExporterInterimSiteQuestionOne)]
    public async Task<IActionResult> InterimSitesQuestionOne(InterimSitesQuestionOneViewModel model, string buttonAction)
    {
        if (!ModelState.IsValid) return View(model);

        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.RegistrationId is null)
        {
            return Redirect("/Error");
        }

        SetBackLink(session, PagePaths.ExporterTaskList);
        await SaveSession(session, PagePaths.ExporterInterimSiteQuestionOne);

        if (buttonAction == SaveAndContinueActionKey)
        {
            if (model.HasInterimSites == true)
            {
                return View("confirm-site1");//this may need to be updated once the page we are redirecting to exists
            }

            await MarkTaskStatusAsCompleted(TaskType.InterimSites);
            return View("tasklist7");//this may need to be updated once the page we are redirecting to exists
        }

        return Redirect(PagePaths.ApplicationSaved);
    }

    private async Task MarkTaskStatusAsCompleted(TaskType taskType)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

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
                await reprocessorService.Registrations.UpdateApplicantRegistrationTaskStatusAsync(registrationId, updateRegistrationTaskStatusDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to call facade for UpdateApplicantRegistrationTaskStatusAsync");
                throw;
            }
        }
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
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
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
    protected RedirectResult ReturnSaveAndContinueRedirect(string buttonAction, string saveAndContinueRedirectUrl, string saveAndComeBackLaterRedirectUrl)
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
    
}