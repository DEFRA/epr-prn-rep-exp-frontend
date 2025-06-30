using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Mapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[Route(PagePaths.RegistrationLanding)]
[FeatureGate(FeatureFlags.ShowRegistration)]
public class ReprocessingInputsAndOutputsController(
	ISessionManager<ReprocessorRegistrationSession> sessionManager,
	IReprocessorService reprocessorService,
    IRegistrationMaterialService registrationMaterialService,
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

		session.RegistrationId = Guid.Parse("84FFEFDC-2306-4854-9B93-4A8A376D7E50");//I will delete this line

		if (session.RegistrationId is null)
		{
			return Redirect(PagePaths.TaskList);
		}

		await SaveSession(session, PagePaths.PackagingWasteWillReprocess);
		SetBackLink(session, PagePaths.PackagingWasteWillReprocess);

		var reprocessingInputsOutputsSession = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs;

		var registrationId = session.RegistrationId;
		var registrationMaterials = await ReprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsAsync(registrationId!.Value);

		if (registrationMaterials.Count > 0)
		{
			reprocessingInputsOutputsSession.Materials = registrationMaterials;
			model.MapForView(registrationMaterials.Select(o => o.MaterialLookup).ToList());
		}

		await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

		return View(nameof(PackagingWasteWillReprocess), model);
	}

    [HttpPost]
    [Route(PagePaths.PackagingWasteWillReprocess)]
    public async Task<IActionResult> PackagingWasteWillReprocess(PackagingWasteWillReprocessViewModel model, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.TaskList, PagePaths.PackagingWasteWillReprocess];

        SetBackLink(session, PagePaths.PackagingWasteWillReprocess);

        var reprocessingInputsOutputs = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs;

        if (!ModelState.IsValid)
        {
            if (reprocessingInputsOutputs.Materials.Count > 0)
            {
                var materials = reprocessingInputsOutputs.Materials.ToList();
                model.MapForView(materials.Select(o => o.MaterialLookup).ToList());
            }

            return View(nameof(PackagingWasteWillReprocess), model);
        }

        reprocessingInputsOutputs.Materials
        .ForEach(p =>
            p.IsMaterialBeingAppliedFor = model.SelectedRegistrationMaterials
                .Contains(p.MaterialLookup.Name.ToString())
        );

        reprocessingInputsOutputs.CurrentMaterial = reprocessingInputsOutputs.Materials!.Find(m => m.IsMaterialBeingAppliedFor == true);

        await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

        //await ReprocessorService.RegistrationMaterials.UpdateIsMaterialRegisteredAsync(reprocessingInputsOutputs.Materials);//changes this before merge to 5.1

        if (buttonAction is SaveAndContinueActionKey)
        {
            if (model.SelectedRegistrationMaterials.Count == reprocessingInputsOutputs.Materials.Count)
            {
                return Redirect(PagePaths.InputsForLastCalendarYear);//changes this before merge to 5.1
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
    [Route(PagePaths.InputsForLastCalendarYear)]
    public async Task<IActionResult> InputLastCalenderYear()
    {

        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

        if (session is null || currentMaterial is null)
        {
            return Redirect(PagePaths.TaskList);
        }

        session.Journey = [PagePaths.PackagingWasteWillReprocess, PagePaths.InputsForLastCalendarYear];//Need to check

        var viewModel = new InputLastCalenderYearViewModel();
        viewModel.MapForView(currentMaterial);//need to check

        SetBackLink(session, PagePaths.InputsForLastCalendarYear);
        await SaveSession(session, PagePaths.InputsForLastCalendarYear);

        return View(nameof(InputLastCalenderYear), viewModel);

    }

    [HttpPost]
    [Route(PagePaths.InputsForLastCalendarYear)]
    public async Task<IActionResult> InputLastCalenderYear(InputLastCalenderYearViewModel viewModel, string action)
    {
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session);
            var currentMaterial = session?.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;

            if (session is null || currentMaterial is null)
            {
                return Redirect(PagePaths.TaskList);
            }

           /*if (!ModelState.IsValid)
            {

                viewModel.MapForView(currentMaterial);//need to check

                SetBackLink(session, PagePaths.InputsForLastCalendarYear);

                return View(nameof(InputLastCalenderYear), viewModel);
            }*/

            //need to check which service need to call
            currentMaterial.RegistrationReprocessingIO ??= new RegistrationReprocessingIODto();//Need to check- this need to be removed
            currentMaterial.RegistrationReprocessingIO.UKPackagingWasteTonne = (decimal?)viewModel?.UkPackagingWaste ?? 0;
            currentMaterial.RegistrationReprocessingIO.NonUKPackagingWasteTonne = (decimal?) viewModel?.NonUkPackagingWaste ?? 0;
            currentMaterial.RegistrationReprocessingIO.NotPackingWasteTonne = (decimal?)viewModel?.NonPackagingWaste ?? 0;
            currentMaterial.RegistrationReprocessingIO.TotalInputs = (decimal?)viewModel?.TotalInputTonnes ?? 0;

            currentMaterial.RegistrationReprocessingIO.RegistrationReprocessingIORawMaterialOrProducts = viewModel.RawMaterials
                .Where(rm => !string.IsNullOrWhiteSpace(rm.RawMaterialName) && rm.Tonnes > 0)
                .Select(rm => new RegistrationReprocessingIORawMaterialOrProductsDto
                {
                    RawMaterialOrProductName = rm.RawMaterialName,
                    TonneValue = (decimal)rm.Tonnes,
                    IsInput = true

                }).ToList();



            await registrationMaterialService.UpsertRegistrationReprocessingDetailsAsync(currentMaterial.Id, currentMaterial.RegistrationReprocessingIO);

            await SaveSession(session, PagePaths.InputsForLastCalendarYear);

            if (action is SaveAndComeBackLaterActionKey)
            {
                return Redirect(PagePaths.ApplicationSaved);
            }

            return RedirectToAction(PagePaths.OutputsForLastCalendarYear, "ReprocessingInputsAndOutputs");
        }
    }

}