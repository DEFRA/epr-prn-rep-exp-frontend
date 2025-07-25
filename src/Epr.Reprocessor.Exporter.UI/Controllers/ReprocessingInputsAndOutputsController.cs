using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;
using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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
    IRequestMapper requestMapper)
    : RegistrationControllerBase(sessionManager, reprocessorService, postcodeLookupService,
        validationService, requestMapper)
{
    [HttpGet]
    [Route(PagePaths.PackagingWasteWillReprocess)]
    public async Task<IActionResult> PackagingWasteWillReprocess()
    {
        var model = new PackagingWasteWillReprocessViewModel();

        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.TaskList, PagePaths.PackagingWasteWillReprocess];

        if (session.RegistrationId is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        await SaveSession(session, PagePaths.PackagingWasteWillReprocess);
        SetBackLink(session, PagePaths.PackagingWasteWillReprocess);

        var reprocessingInputsOutputsSession = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs;

        var registrationId = session.RegistrationId;
        var registrationMaterials =
            await ReprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(registrationId!.Value);

        if (registrationMaterials.Count > 0)
        {
            reprocessingInputsOutputsSession.Materials = registrationMaterials;
            if (registrationMaterials.Count == 1)
            {
                reprocessingInputsOutputsSession.CurrentMaterial = reprocessingInputsOutputsSession.Materials!.FirstOrDefault();
                reprocessingInputsOutputsSession.CurrentMaterial.IsMaterialBeingAppliedFor = true;
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

            return Redirect(PagePaths.MaterialNotReprocessingReason);
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
          
      return Redirect(PagePaths.ReprocessingInputs);
        
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
            var materialName = currentMaterial.MaterialLookup.DisplayText;
            viewModel.MapForView(viewModel.TypeOfSuppliers, materialName);
            SetBackLink(session, PagePaths.TypeOfSuppliers);

            return View(nameof(TypeOfSuppliers), viewModel);
        }

        currentMaterial.RegistrationReprocessingIO ??= new RegistrationReprocessingIODto();
        currentMaterial.RegistrationReprocessingIO.TypeOfSuppliers = viewModel.TypeOfSuppliers;

        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);
        await SaveSession(session, PagePaths.TypeOfSuppliers);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("LastCalendarYearFlag", "ReprocessingInputsAndOutputs");
    }

    [HttpGet]
    [Route(PagePaths.MaterialNotReprocessingReason)]
    public async Task<IActionResult> MaterialNotReprocessingReason([FromQuery] Guid? materialId)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        var allUncheckedMaterials = GetAllUncheckedMaterials(session);
        var firstUncheckedMaterial = materialId is null ? allUncheckedMaterials.FirstOrDefault() : allUncheckedMaterials.Find(x => x.Id == materialId);
        if (firstUncheckedMaterial == null)
        {
            return Redirect(PagePaths.ApplicationContactName);
        }

        var viewModel = new MaterialNotReprocessingReasonModel();
        viewModel.MapForView(firstUncheckedMaterial.Id, firstUncheckedMaterial.MaterialNotReprocessingReason, firstUncheckedMaterial.MaterialLookup.DisplayText);

        SetJourneyForMaterialNotReprocessingReason(session, allUncheckedMaterials, firstUncheckedMaterial.Id);
        SetBackLink(session, PagePaths.MaterialNotReprocessingReason);
        await SaveSession(session, PagePaths.MaterialNotReprocessingReason);

        return View(nameof(MaterialNotReprocessingReason), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.MaterialNotReprocessingReason)]
    public async Task<IActionResult> MaterialNotReprocessingReason(MaterialNotReprocessingReasonModel viewModel, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        var allUncheckedMaterials = GetAllUncheckedMaterials(session);
        var currentMaterial = allUncheckedMaterials.FirstOrDefault(x => x.Id == viewModel.MaterialId);

        if (currentMaterial == null)
        {
            return Redirect(PagePaths.ApplicationContactName);
        }

        if (!ModelState.IsValid)
        {
            viewModel.MapForView(currentMaterial.Id, viewModel.MaterialNotReprocessingReason, currentMaterial.MaterialLookup.DisplayText);
            SetBackLink(session, PagePaths.MaterialNotReprocessingReason);

            return View(nameof(MaterialNotReprocessingReason), viewModel);
        }

        currentMaterial.MaterialNotReprocessingReason = viewModel.MaterialNotReprocessingReason;
        await registrationMaterialService.UpdateMaterialNotReprocessingReasonAsync(currentMaterial.Id, viewModel.MaterialNotReprocessingReason);
        await SaveSession(session, PagePaths.MaterialNotReprocessingReason);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        if (!TryGetNextMaterial(allUncheckedMaterials, currentMaterial, out var nextMaterial))
        {
            return Redirect(PagePaths.ApplicationContactName);
        }

        return RedirectToAction("MaterialNotReprocessingReason", "ReprocessingInputsAndOutputs", new { materialId = nextMaterial.Id });
    }

    [HttpGet]
    [Route(PagePaths.ReprocessingInputs)]
    public async Task<IActionResult> ReprocessingInputs()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.LastCalendarYearFlag, PagePaths.ReprocessingInputs];

        var viewModel = new ReprocessingInputsViewModel();
        viewModel.MapForView(currentMaterial);
        viewModel.InputsLastCalendarYearFlag = currentMaterial.RegistrationReprocessingIO?.ReprocessingPackagingWasteLastYearFlag ?? false;

        SetBackLink(session, PagePaths.ReprocessingInputs);
        await SaveSession(session, PagePaths.ReprocessingInputs);

        return View(nameof(ReprocessingInputs), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.ReprocessingInputs)]
    public async Task<IActionResult> ReprocessingInputs(ReprocessingInputsViewModel viewModel, string buttonAction)
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
            SetBackLink(session, PagePaths.ReprocessingInputs);
            return View(nameof(ReprocessingInputs), viewModel);
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
        await SaveSession(session, PagePaths.ReprocessingInputs);

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

        session.Journey = [PagePaths.ReprocessingInputs, PagePaths.OutputsForLastCalendarYear];

        await SaveSession(session, PagePaths.OutputsForLastCalendarYear);
        SetBackLink(session, PagePaths.OutputsForLastCalendarYear);

        var materialoutput = new ReprocessedMaterialOutputSummaryModel()
        {
            MaterialName = currentMaterial.MaterialLookup.Name.ToString().ToLower(),
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>()
        };
        for (int i = 0; i < 10; i++)
        {
            materialoutput.ReprocessedMaterialsRawData.Add(new ReprocessedMaterialRawDataModel());
        }

        if (currentMaterial.RegistrationReprocessingIO.ReprocessingPackagingWasteLastYearFlag)
        {
            return View(nameof(ReprocessingOutputsForLastYear), materialoutput);
        }
        else
        {
            return Redirect(PagePaths.OutputsForEstimate);
        }

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
        var reprocessingOutputs = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO ?? new RegistrationReprocessingIODto();

        var validationResult = await ValidationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            SetBackLink(session, PagePaths.OutputsForLastCalendarYear);

            model.MaterialName ??= currentMaterial.MaterialLookup.Name.ToString().ToLower();

            return View(nameof(ReprocessingOutputsForLastYear), model);
        }
        reprocessingOutputs.SenttoOtherSiteTonne = decimal.TryParse(model.SentToOtherSiteTonnes, out var SentToOtherSiteTonnes) ? SentToOtherSiteTonnes : 0;
        reprocessingOutputs.ContaminantsTonne = decimal.TryParse(model.ContaminantTonnes, out var ContaminantTonnes) ? ContaminantTonnes : 0;

        reprocessingOutputs.ProcessLossTonne = decimal.TryParse(model.ProcessLossTonnes, out var ProcessLossTonnes) ? ProcessLossTonnes : 0;

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

    [HttpGet]
    [Route(PagePaths.PlantAndEquipment)]
    public async Task<IActionResult> PlantAndEquipment()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        var viewModel = new PlantAndEquipmentViewModel();
        viewModel.MapForView(currentMaterial.MaterialLookup.Name.GetDisplayName(), currentMaterial.RegistrationReprocessingIO.PlantEquipmentUsed);

        session.Journey = [PagePaths.OutputsForLastCalendarYear, PagePaths.PlantAndEquipment];
        SetBackLink(session, PagePaths.PlantAndEquipment);

        await SaveSession(session, PagePaths.PlantAndEquipment);

        return View(nameof(PlantAndEquipment), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.PlantAndEquipment)]
    public async Task<IActionResult> PlantAndEquipment(PlantAndEquipmentViewModel viewModel, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        if (!ModelState.IsValid)
        {
            viewModel.MapForView(currentMaterial.MaterialLookup.Name.GetDisplayName(), viewModel.PlantEquipmentUsed);

            session.Journey = [PagePaths.OutputsForLastCalendarYear, PagePaths.PlantAndEquipment];
            SetBackLink(session, PagePaths.PlantAndEquipment);
            await SaveSession(session, PagePaths.PlantAndEquipment);

            return View(nameof(PlantAndEquipment), viewModel);
        }

        currentMaterial.RegistrationReprocessingIO.PlantEquipmentUsed = viewModel.PlantEquipmentUsed;

        await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);

        await SaveSession(session, PagePaths.ApplicationContactName);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("ReviewAnswers", "ReprocessingInputsAndOutputs");
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

    private static void SetJourneyForApplicationContactName(ReprocessorRegistrationSession session)
    {
        var materials = GetMaterials(session);
        var lastUncheckedMaterial = materials?.Where(x => x.IsMaterialBeingAppliedFor == false).OrderBy(x => x.MaterialLookup.DisplayText).LastOrDefault();
        if(lastUncheckedMaterial == null)
        {
            if (materials?.Count == 1)
            {
                session.Journey = [PagePaths.TaskList, PagePaths.ApplicationContactName];
            }
            else
            {
                session.Journey = [PagePaths.PackagingWasteWillReprocess, PagePaths.ApplicationContactName];
            }
        }
        else
        {
            var backPath = $"{PagePaths.MaterialNotReprocessingReason}?materialId={lastUncheckedMaterial.Id}";
            session.Journey = [backPath, PagePaths.ApplicationContactName];
        }
    }

    private static void SetJourneyForMaterialNotReprocessingReason(ReprocessorRegistrationSession session, List<RegistrationMaterialDto> allUncheckedMaterials, Guid firstUncheckedMaterialId)
    {
        var currentIndex = allUncheckedMaterials.FindIndex(x => x.Id == firstUncheckedMaterialId);
        var backPath = PagePaths.PackagingWasteWillReprocess;
        if (currentIndex != 0)
        {
            var previousItemId = allUncheckedMaterials[currentIndex - 1].Id;
            backPath = $"{PagePaths.MaterialNotReprocessingReason}?materialId={previousItemId}";
        }
        session.Journey = [backPath, PagePaths.MaterialNotReprocessingReason];
    }

    private bool TryGetNextMaterial(List<RegistrationMaterialDto> materials, RegistrationMaterialDto current, out RegistrationMaterialDto next)
    {
        next = null;
        if (materials == null || current == null)
            return false;

        var index = materials.FindIndex(x => x.Id == current.Id);
        if (index < 0 || index >= materials.Count - 1)
            return false;

        next = materials[index + 1];
        return true;
    }

    private static List<RegistrationMaterialDto> GetMaterials(ReprocessorRegistrationSession session) => session?.RegistrationApplicationSession?.ReprocessingInputsAndOutputs?.Materials;
    private static List<RegistrationMaterialDto> GetAllUncheckedMaterials(ReprocessorRegistrationSession session) => GetMaterials(session)?.Where(x => x.IsMaterialBeingAppliedFor == false).OrderBy(x => x.MaterialLookup.DisplayText).ToList() ?? [];
}