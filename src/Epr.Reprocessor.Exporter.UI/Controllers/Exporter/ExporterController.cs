using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IMapper mapper,
    IRegistrationService registrationService,
    IExporterRegistrationService exporter) : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
    protected const string NoOverseasReprocessorSiteError = "You must have at least one overseas reprocessors site before you can continue";

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

        await SetTempBackLink(PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);


        SetBackLink(session, PagePaths.OverseasSiteDetails);
        await SaveSession(session, PagePaths.OverseasSiteDetails);
        return ReturnSaveAndContinueRedirect(buttonAction, PagePaths.BaselConventionAndOECDCodes, PagePaths.ApplicationSaved);
    }

    [HttpGet]
    [Route(PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task<IActionResult> CheckOverseasReprocessingSitesAnswers([FromQuery] string? buttonAction)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session) ?? new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
        };

        if (session.ExporterRegistrationApplicationSession == null)
        {
            session.ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession();
        }

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites ??= PopulateOverseasAddresses();
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId ??= Guid.NewGuid();

        //var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite];

        if (session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count == 0 && !string.IsNullOrEmpty(buttonAction))
        {
            var modelError = new CheckOverseasReprocessingSitesAnswersViewModel(session.ExporterRegistrationApplicationSession);

            ModelState.AddModelError(nameof(modelError.OverseasAddresses), NoOverseasReprocessorSiteError);
            return View("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml", modelError);
        }

        SetBackLink(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);
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

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);
            return ReturnSaveAndContinueRedirect(buttonAction, string.Empty, PagePaths.ApplicationSaved);
        }

        if (session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count == 0
            && buttonAction == SaveAndContinueActionKey)
        {
            return RedirectToAction("CheckOverseasReprocessingSitesAnswers", new { buttonAction = buttonAction });
        }

        SetBackLink(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);
        await SaveSession(session, PagePaths.CheckYourAnswersForOverseasProcessingSite);

        await exporter.SaveOverseasReprocessorAsync(mapper.Map<OverseasAddressRequestDto>(session.ExporterRegistrationApplicationSession));
        return Redirect(PagePaths.RegistrationLanding);
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

        return RedirectToAction("Index");
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

        return RedirectToAction("CheckOverseasReprocessingSitesAnswers");
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

        // TODO: replace with actual logic to change Basel Convention codes
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route(PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task<IActionResult> AddAnotherOverseasReprocessingSite()
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        var overseasAddresses = session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();

        for (int i = 0; i < overseasAddresses.Count; i++)
        {
            overseasAddresses[i].IsActive = false;
        }

        await SaveSession(session, PagePaths.AddAnotherOverseasReprocessingSite);

        return RedirectToAction("Index");
    }

    public static OverseasReprocessingSites PopulateOverseasAddresses()
    {
        return new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress>
        {
            new OverseasAddress
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Suite 100",
                CityorTown = "London",
                Country = "United Kingdom",
                Id = Guid.NewGuid(),
                OrganisationName = "Acme Reprocessing Ltd",
                PostCode = "AB12 3CD",
                SiteCoordinates = "51.5074,-0.1278",
                StateProvince = "Greater London",
                IsActive = true,
                OverseasAddressContact = new List<OverseasAddressContact>
                {
                    new()
                    {
                        FullName = "Dan Export",
                        Email = "danexport@dan.com",
                        PhoneNumber = "020 1234 5678",
                    }
                },
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                {
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        CodeName = "C10512",
                    }
                }
            },
            new OverseasAddress
            {
                AddressLine1 = "456 Elm St",
                AddressLine2 = "Floor 2",
                CityorTown = "Paris",
                Country = "France",
                Id = Guid.NewGuid(),
                OrganisationName = "Reprocess Europe SARL",
                PostCode = "75001",
                SiteCoordinates = "48.8566,2.3522",
                StateProvince = "Île-de-France",
                IsActive = false,
                OverseasAddressContact = new List<OverseasAddressContact>
                {
                    new()
                    {
                        FullName = "Dan Export",
                        Email = "danexport@dan.com",
                        PhoneNumber = "020 1234 5678",
                    }
                },
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                {
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        CodeName = "B1001",
                    }
                }
            },
            new OverseasAddress
            {
                AddressLine1 = "456 Elm St",
                AddressLine2 = "Floor 2",
                CityorTown = "Paris",
                Country = "France",
                Id = Guid.NewGuid(),
                OrganisationName = "Process Europe SARL",
                PostCode = "75001",
                SiteCoordinates = "48.8566,2.3522",
                StateProvince = "Île-de-France",
                IsActive = false,
                OverseasAddressContact = new List<OverseasAddressContact>
                {
                    new()
                    {
                        FullName = "Dan Export",
                        Email = "danexport@dan.com",
                        PhoneNumber = "020 1234 5678",
                    }
                },
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                {
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        CodeName = "B1001",
                    }
                }
            }
        }
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