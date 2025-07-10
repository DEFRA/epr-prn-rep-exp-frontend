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

        // Ensure ExporterRegistrationApplicationSession is not null
        session = session ?? new ExporterRegistrationSession();
        session.ExporterRegistrationApplicationSession ??= new ExporterRegistrationApplicationSession();

        // Check if InterimSites is null or has no sites, then create sample
        if (session.ExporterRegistrationApplicationSession.InterimSites == null ||
            session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites == null ||
            session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Count == 0)
        {
            session.ExporterRegistrationApplicationSession.InterimSites = CreateSampleInterimSites();
        }

        var activeOverseasSite = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites.Find(x => x.IsActive);

        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.ExporterInterimSitesUsed];
        SetBackLink(session, PagePaths.ExporterInterimSitesUsed);

        //if (session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count == 0 && !string.IsNullOrEmpty(buttonAction))
        //{
        //    var modelError = new CheckInterimSitesAnswersViewModel(activeOverseasSite);

        //    ModelState.AddModelError(nameof(modelError.OverseasAddresses), NoOverseasReprocessorSiteError);
        //    return View("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml", modelError);
        //}

        await SaveSession(session, PagePaths.ExporterInterimSitesUsed);

        var model = new CheckInterimSitesAnswersViewModel(activeOverseasSite);

        return View("~/Views/Registration/Exporter/CheckInterimSitesAnswers.cshtml", model);
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

        return RedirectToAction(nameof(Index)); // redirect to Mark's method
    }

    [HttpGet]
    [Route(PagePaths.DeleteInterimSite)]
    public async Task<IActionResult> DeleteInterimSite([FromQuery] int index)
    {
        var session = await sessionManager.GetSessionAsync(HttpContext.Session);
        session.Journey = [PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.ExporterInterimSitesUsed];

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

        return RedirectToAction(nameof(Index));
    }

    public static InterimSites CreateSampleInterimSites()
    {
        var site1 = new OverseasMaterialReprocessingSite
        {
            IsActive = true,
            OverseasAddress = new OverseasAddressBase
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Suite 100",
                CityorTown = "London",
                Country = "UK",
                Id = Guid.NewGuid(),
                OrganisationName = "Org One",
                PostCode = "W1A 1AA",
                StateProvince = "Greater London"
            },
            InterimSiteAddresses = new List<InterimSiteAddress>
         {
             new InterimSiteAddress
             {
                 Id = Guid.NewGuid(),
                 IsActive = true,
                 OrganisationName = "Sample Organisation",
                 AddressLine1 = "123 Example Street",
                 AddressLine2 = "Suite 456",
                 CityorTown = "Sample City",
                 StateProvince = "Sample State",
                 Country = "Sample Country",
                 PostCode = "AB12 3CD",
                 InterimAddressContact = new List<OverseasAddressContact>
                 {
                     new OverseasAddressContact
                     {
                         FullName = "John Doe",
                         Email = "john.doe@example.com",
                         PhoneNumber = "+1234567890"
                     },
                     new OverseasAddressContact
                     {
                         FullName = "Jane Smith",
                         Email = "jane.smith@example.com",
                         PhoneNumber = "+0987654321"
                     }
                 }
             },
             new InterimSiteAddress
             {
                 Id = Guid.NewGuid(),
                 IsActive = false,
                 OrganisationName = "Sample Organisation 2",
                 AddressLine1 = "123 Example Street 2",
                 AddressLine2 = "Suite 456 2",
                 CityorTown = "Sample City 2",
                 StateProvince = "Sample State 2",
                 Country = "Sample Country 2",
                 PostCode = "AB12 3CD 2",
                 InterimAddressContact = new List<OverseasAddressContact>
                 {
                     new OverseasAddressContact
                     {
                         FullName = "John Doe 2",
                         Email = "john.doe2@example.com",
                         PhoneNumber = "+12345678902"
                     },
                     new OverseasAddressContact
                     {
                         FullName = "Jane Smith2",
                         Email = "jane.smith2@example.com",
                         PhoneNumber = "+09876543212"
                     }
                 }
             }
         }
        };

        var site2 = new OverseasMaterialReprocessingSite
        {
            IsActive = false,
            OverseasAddress = new OverseasAddressBase
            {
                AddressLine1 = "456 Second Ave",
                AddressLine2 = "Floor 2",
                CityorTown = "Manchester",
                Country = "UK",
                Id = Guid.NewGuid(),
                OrganisationName = "Org Two",
                PostCode = "M1 2AB",
                StateProvince = "Greater Manchester"
            },
            InterimSiteAddresses = new List<InterimSiteAddress>
         {
             new InterimSiteAddress
             {
                 Id = Guid.NewGuid(),
                 IsActive = true,
                 OrganisationName = "Sample Organisation 2",
                 AddressLine1 = "123 Example Street 2",
                 AddressLine2 = "Suite 456 2",
                 CityorTown = "Sample City 2",
                 StateProvince = "Sample State 2",
                 Country = "Sample Country 2",
                 PostCode = "AB12 3CD 2",
                 InterimAddressContact = new List<OverseasAddressContact>
                 {
                     new OverseasAddressContact
                     {
                         FullName = "John Doe 1",
                         Email = "john.doe1@example.com",
                         PhoneNumber = "+987984567890"
                     },
                     new OverseasAddressContact
                     {
                         FullName = "Jane Smith1",
                         Email = "jane.smith1@example.com",
                         PhoneNumber = "+980890987654321"
                     }
                 }
             }
         }
        };

        return new InterimSites
        {
            OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { site1, site2 }
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