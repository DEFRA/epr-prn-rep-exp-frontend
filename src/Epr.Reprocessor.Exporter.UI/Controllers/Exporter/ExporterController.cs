using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IMapper mapper,
    IRegistrationService registrationService,
    IValidationService validationService) : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

    [HttpGet]
    [Route(PagePaths.OverseasSiteDetails)]
    public async Task<IActionResult> Index()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            return Redirect("/Error");
        }

        session.Journey = ["test-setup-session", PagePaths.OverseasSiteDetails];

        var activeOverseasAddress = session.ExporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses?.SingleOrDefault(oa => oa.IsActive);

        OverseasReprocessorSiteViewModel model;
        if (activeOverseasAddress != null)
        {
            model = mapper.Map<OverseasReprocessorSiteViewModel>(activeOverseasAddress);
        }
        else
        { 
            model = new OverseasReprocessorSiteViewModel();
            model.IsFirstSite = true;
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

        var overseasReprocessingSites = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites;

        if (overseasReprocessingSites == null)
        {
            overseasReprocessingSites = new OverseasReprocessingSites();
            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites = overseasReprocessingSites;
        }
        if (overseasReprocessingSites.OverseasAddresses == null)
        {
            overseasReprocessingSites.OverseasAddresses = new List<OverseasAddress>();
        }

        var activeOverseasAddress = overseasReprocessingSites.OverseasAddresses.SingleOrDefault(oa => oa.IsActive);

        if (activeOverseasAddress != null)
        {
            mapper.Map(model, activeOverseasAddress);
        }
        else
        {
            var overseasAddress = mapper.Map<OverseasAddress>(model);
            overseasAddress.IsActive = true;
            overseasReprocessingSites.OverseasAddresses.Add(overseasAddress);
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
        var validationResult = await validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        await SetTempBackLink(PagePaths.BaselConventionAndOECDCodes, PagePaths.AddAnotherOverseasReprocessingSite);

        var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession();

        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        overseasAddresses.ForEach(a => a.IsActive = false);

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);


        if (model.AddOverseasSiteAccepted == true)
        {
            return Redirect(PagePaths.OverseasSiteDetails);
        }
        else if (model.AddOverseasSiteAccepted == false)
        {
            return Redirect(PagePaths.CheckYourAnswersOverseasReprocessor);
        }     

        return View(model);
    }
}