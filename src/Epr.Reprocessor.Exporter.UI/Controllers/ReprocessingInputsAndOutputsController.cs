using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[Route(PagePaths.RegistrationLanding)]
[FeatureGate(FeatureFlags.ShowRegistration)]
public class ReprocessingInputsAndOutputsController(
    ISessionManager<ReprocessorRegistrationSession> sessionManager,
    IRegistrationMaterialService registrationMaterialService,
    IAccountServiceApiClient accountService,
    IReprocessorService reprocessorService,
    IPostcodeLookupService postcodeLookupService,
    IValidationService validationService,
    IStringLocalizer<SelectAuthorisationType> selectAuthorisationStringLocalizer,
    IRequestMapper requestMapper)
    : RegistrationControllerBase(sessionManager, reprocessorService, postcodeLookupService,
        validationService, selectAuthorisationStringLocalizer, requestMapper)
{
    [HttpGet]
    [Route(PagePaths.PackagingWasteWillReprocess)]
    public async Task<IActionResult> PackagingWasteWillReprocess()
    {
        var model = new PackagingWasteWillReprocessViewModel();

        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.TaskList, PagePaths.PackagingWasteWillReprocess];

        // TODO: This line to be deleted once the first two steps are working correctly.
        // Currently, we need this for testing.
        session.RegistrationId = Guid.Parse("3B90C092-C10E-450A-92AE-F3DF455D2D95");

        await SaveSession(session, PagePaths.PackagingWasteWillReprocess);
        SetBackLink(session, PagePaths.PackagingWasteWillReprocess);

        var reprocessingInputsOutputsSession = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs;

        var registrationId = session.RegistrationId;
        var registrationMaterials =
            await ReprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsAsync(registrationId!.Value);

        if (registrationMaterials.Count > 0)
        {
            reprocessingInputsOutputsSession.Materials = registrationMaterials;
            if(registrationMaterials.Count == 1)
            {
                reprocessingInputsOutputsSession.CurrentMaterial = reprocessingInputsOutputsSession.Materials!.FirstOrDefault();
				await SaveSession(session, PagePaths.PackagingWasteWillReprocess);
                await ReprocessorService.RegistrationMaterials.UpdateIsMaterialRegisteredAsync(reprocessingInputsOutputsSession.Materials);
				return Redirect(PagePaths.ApplicationContactName);
			}
			model.MapForView(registrationMaterials.Select(o => o.MaterialLookup).ToList());
		}

        await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

        return View(nameof(PackagingWasteWillReprocess), model);
    }

    [HttpPost]
    [Route(PagePaths.PackagingWasteWillReprocess)]
    public async Task<IActionResult> PackagingWasteWillReprocess(PackagingWasteWillReprocessViewModel model,
        string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.TaskList, PagePaths.PackagingWasteWillReprocess];

        SetBackLink(session, PagePaths.PackagingWasteWillReprocess);

        var reprocessingInputsOutputs = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs;

        var validationResult = await ValidationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            if (reprocessingInputsOutputs.Materials.Count > 0)
            {
                var materials = reprocessingInputsOutputs.Materials.ToList();
                model.MapForView(materials.Select(o => o.MaterialLookup).ToList());
            }

            return View(nameof(PackagingWasteWillReprocess), model);
        }

        if (model.SelectedRegistrationMaterials.Count > 0)
        {
            reprocessingInputsOutputs.Materials
                .ForEach(p =>
                    p.IsMaterialBeingAppliedFor = model.SelectedRegistrationMaterials
                        .Contains(p.MaterialLookup.Name.ToString())
                );
        }

        reprocessingInputsOutputs.CurrentMaterial =
            reprocessingInputsOutputs.Materials!.Find(m => m.IsMaterialBeingAppliedFor == true);

        await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

        await ReprocessorService.RegistrationMaterials.UpdateIsMaterialRegisteredAsync(reprocessingInputsOutputs
            .Materials);

        if (buttonAction is SaveAndContinueActionKey)
        {
            if (model.SelectedRegistrationMaterials.Count == reprocessingInputsOutputs.Materials.Count)
            {
                return Redirect(PagePaths.ApplicationContactName);
            }

            return Redirect(PagePaths.ReasonNotReprocessing);
        }

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }

    [HttpGet]
    [Route(PagePaths.LastCalendarYearFlag)]
    public async Task<IActionResult> LastCalendarYearFlag()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.TypeOfSuppliers, PagePaths.LastCalendarYearFlag];

        var viewModel = new LastCalendarYearFlagViewModel();
        viewModel.Year = DateTime.Now.Year - 1;
        viewModel.TypeOfWaste = $"{currentMaterial.MaterialLookup.Name}";

        SetBackLink(session, PagePaths.LastCalendarYearFlag);
        await SaveSession(session, PagePaths.LastCalendarYearFlag);

        return View(nameof(LastCalendarYearFlag), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.LastCalendarYearFlag)]
    public async Task<IActionResult> LastCalendarYearFlag(LastCalendarYearFlagViewModel model,
        string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        if (!ModelState.IsValid)
        {
            SetBackLink(session, PagePaths.LastCalendarYearFlag);

            model.Year = DateTime.Now.Year - 1;
            model.TypeOfWaste = $"{currentMaterial.MaterialLookup.Name}";

            return View(nameof(LastCalendarYearFlag), model);
        }

        currentMaterial.RegistrationReprocessingIO ??= new RegistrationReprocessingIODto();
        currentMaterial.RegistrationReprocessingIO.ReprocessingPackagingWasteLastYearFlag = model.ReprocessingPackagingWasteLastYearFlag.Value;

        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);
        await SaveSession(session, PagePaths.LastCalendarYearFlag);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        if(model.ReprocessingPackagingWasteLastYearFlag == true)
        {
            return Redirect(PagePaths.InputsForLastCalendarYear);
        }
        else
        {
            return Redirect(PagePaths.EstimateAnnualInputs);
        }
    }

    [HttpGet]
    [Route(PagePaths.ApplicationContactName)]
    public async Task<IActionResult> ApplicationContactName()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        var userData = User.GetUserData();
        var organisationPersons = await GetOrganisationPersons(userData);

        var viewModel = new ApplicationContactNameViewModel();
        viewModel.MapForView(userData.Id, currentMaterial, organisationPersons);

        SetJourneyForApplicationContactName(session);
        SetBackLink(session, PagePaths.ApplicationContactName);

        await SaveSession(session, PagePaths.ApplicationContactName);

        return View(nameof(ApplicationContactName), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.ApplicationContactName)]
    public async Task<IActionResult> ApplicationContactName(ApplicationContactNameViewModel viewModel,
        string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        if (!ModelState.IsValid)
        {
            var userData = User.GetUserData();
            var organisationPersons = await GetOrganisationPersons(userData);

            viewModel.MapForView(userData.Id, currentMaterial, organisationPersons);

            SetBackLink(session, PagePaths.ApplicationContactName);

            return View(nameof(ApplicationContactName), viewModel);
        }

        currentMaterial.RegistrationMaterialContact.UserId = viewModel.SelectedContact!.Value;

        currentMaterial.RegistrationMaterialContact =
            await registrationMaterialService.UpsertRegistrationMaterialContactAsync(
                currentMaterial.Id, currentMaterial.RegistrationMaterialContact);

        await SaveSession(session, PagePaths.ApplicationContactName);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("TypeOfSuppliers", "ReprocessingInputsAndOutputs"); 
    }

    [HttpGet]
    [Route(PagePaths.TypeOfSuppliers)]
    public async Task<IActionResult> TypeOfSuppliers()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.ApplicationContactName, PagePaths.TypeOfSuppliers];

        var typeOfSuppliers = currentMaterial.RegistrationReprocessingIO?.TypeOfSuppliers;
        var materialName = currentMaterial.MaterialLookup.DisplayText;

        var viewModel = new TypeOfSuppliersViewModel();
        viewModel.MapForView(typeOfSuppliers, materialName);

        SetBackLink(session, PagePaths.TypeOfSuppliers);
        await SaveSession(session, PagePaths.TypeOfSuppliers);

        return View(nameof(TypeOfSuppliers), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.TypeOfSuppliers)]
    public async Task<IActionResult> TypeOfSuppliers(TypeOfSuppliersViewModel viewModel, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        if (!ModelState.IsValid)
        {
            var typeOfSuppliers = currentMaterial.RegistrationReprocessingIO?.TypeOfSuppliers;
            var materialName = currentMaterial.MaterialLookup.DisplayText;

            viewModel.MapForView(typeOfSuppliers, materialName);

            SetBackLink(session, PagePaths.ApplicationContactName);

            return View(nameof(TypeOfSuppliers), viewModel);
        }

        currentMaterial.RegistrationReprocessingIO ??= new RegistrationReprocessingIODto();
        currentMaterial.RegistrationReprocessingIO.TypeOfSuppliers = viewModel.TypeOfSuppliers;

        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);
        await SaveSession(session, PagePaths.ApplicationContactName);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("LastCalendarYearFlag", "ReprocessingInputsAndOutputs");
    }

    [HttpGet]
    [Route(PagePaths.InputsForLastCalendarYear)]
    public async Task<IActionResult> InputsForLastCalendarYear()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.LastCalendarYearFlag, PagePaths.InputsForLastCalendarYear];

        var viewModel = new InputsForLastCalendarYearViewModel();
        viewModel.MapForView(currentMaterial);

        SetBackLink(session, PagePaths.InputsForLastCalendarYear);
        await SaveSession(session, PagePaths.InputsForLastCalendarYear);

        return View(nameof(InputsForLastCalendarYear), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.InputsForLastCalendarYear)]
    public async Task<IActionResult> InputsForLastCalendarYear(InputsForLastCalendarYearViewModel viewModel, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        var validationResult = await ValidationService.ValidateAsync(viewModel);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            viewModel.MapForView(currentMaterial);
            SetBackLink(session, PagePaths.InputsForLastCalendarYear);
            return View(nameof(InputsForLastCalendarYear), viewModel);
        }

        currentMaterial.RegistrationReprocessingIO ??= new RegistrationReprocessingIODto();
        currentMaterial.RegistrationReprocessingIO.UKPackagingWasteTonne = decimal.TryParse(viewModel?.UkPackagingWaste, out var uk) ? uk : 0;
        currentMaterial.RegistrationReprocessingIO.NonUKPackagingWasteTonne = decimal.TryParse(viewModel?.NonUkPackagingWaste, out var nonUk) ? nonUk : 0;
        currentMaterial.RegistrationReprocessingIO.NotPackingWasteTonne = decimal.TryParse(viewModel?.NonPackagingWaste, out var nonPack) ? nonPack : 0;
        currentMaterial.RegistrationReprocessingIO.TotalInputs = viewModel?.TotalInputTonnes ?? 0;

        currentMaterial.RegistrationReprocessingIO.RegistrationReprocessingIORawMaterialOrProducts = (viewModel?.RawMaterials ?? new List<RawMaterialRowViewModel>())
           .Where(rm => !string.IsNullOrWhiteSpace(rm.RawMaterialName) && !string.IsNullOrWhiteSpace(rm.Tonnes))
           .Select(rm => new RegistrationReprocessingIORawMaterialOrProductsDto
           {
               RawMaterialOrProductName = rm.RawMaterialName,
               TonneValue = decimal.TryParse(rm.Tonnes, out var tonnes) ? tonnes : 0,
               IsInput = true
           }).ToList();

        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);
        await SaveSession(session, PagePaths.InputsForLastCalendarYear);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("ReprocessingOutputsForLastYear", "ReprocessingInputsAndOutputs");

    }

    [HttpGet]
    [Route(PagePaths.OutputsForLastCalendarYear)]
    public async Task<IActionResult> ReprocessingOutputsForLastYear()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.InputsForLastCalendarYear, PagePaths.OutputsForLastCalendarYear];

        await SaveSession(session, PagePaths.OutputsForLastCalendarYear);
        SetBackLink(session, PagePaths.OutputsForLastCalendarYear);
        
        var materialoutput = new ReprocessedMaterialOutputSummaryModel()
        {
            MaterialName = currentMaterial.MaterialLookup.Name.ToString(),            
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>()
        };
        for (int i = 0; i < 10; i++)
        {
            materialoutput.ReprocessedMaterialsRawData.Add(new ReprocessedMaterialRawDataModel());
        }

        return View(nameof(ReprocessingOutputsForLastYear), materialoutput);

    }
    [HttpPost]
    [Route(PagePaths.OutputsForLastCalendarYear)]
    public async Task<IActionResult> ReprocessingOutputsForLastYear(ReprocessedMaterialOutputSummaryModel model, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;
        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }
        var reprocessingOutputs = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO??new RegistrationReprocessingIODto();

        var validationResult = await ValidationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            SetBackLink(session, PagePaths.OutputsForLastCalendarYear);
            return View(nameof(ReprocessingOutputsForLastYear), model);
        }
        reprocessingOutputs.SenttoOtherSiteTonne = decimal.TryParse(model.SentToOtherSiteTonnes.ToString(), out var SentToOtherSiteTonnes) ? SentToOtherSiteTonnes : 0; 
        reprocessingOutputs.ContaminantsTonne = decimal.TryParse(model.ContaminantTonnes.ToString(), out var ContaminantTonnes) ? ContaminantTonnes : 0;
       
        reprocessingOutputs.ProcessLossTonne = decimal.TryParse(model.ProcessLossTonnes.ToString(), out var ProcessLossTonnes) ? ProcessLossTonnes : 0;
            
        reprocessingOutputs.RegistrationReprocessingIORawMaterialOrProducts = model.ReprocessedMaterialsRawData
            .Where(rm => !string.IsNullOrWhiteSpace(rm.MaterialOrProductName) && !string.IsNullOrWhiteSpace(rm.ReprocessedTonnes))
            .Select(rm => new RegistrationReprocessingIORawMaterialOrProductsDto
            {
                TonneValue = decimal.TryParse(rm.ReprocessedTonnes, out var tonnes) ? tonnes : 0,
                RawMaterialOrProductName = rm.MaterialOrProductName,
                IsInput = false
            }).ToList();
            reprocessingOutputs.TotalOutputs = decimal.TryParse(model.TotalOutputTonnes.ToString(), out var tonnes) ? tonnes : 0; 
        
        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);

        await SaveSession(session, PagePaths.OutputsForLastCalendarYear);

        if (buttonAction is SaveAndContinueActionKey)
        {
            return Redirect(PagePaths.PlantAndEquipment);
        }

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }

    private async Task<IEnumerable<OrganisationPerson>> GetOrganisationPersons(UserData userData)
    {
        var organisationId = userData.Organisations[0].Id;

        if (organisationId.HasValue == false || organisationId == Guid.Empty)
        {
            return [];
        }

        var organisationDetails = await accountService.GetOrganisationDetailsAsync(organisationId.Value);

        return organisationDetails?.Persons.Where(p => p.UserId != userData.Id) ?? [];
    }

    private void SetJourneyForApplicationContactName(ReprocessorRegistrationSession session)
    {
        var materialCount = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Materials.Count;

        if (materialCount == 1)
        {
            session.Journey = [PagePaths.TaskList, PagePaths.ApplicationContactName];
        }
        else
        {
            session.Journey = [PagePaths.PackagingWasteWillReprocess, PagePaths.ApplicationContactName];
        }
    }
}