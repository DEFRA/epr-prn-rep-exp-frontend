using Epr.Reprocessor.Exporter.UI.Mapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[Route(PagePaths.RegistrationLanding)]
[FeatureGate(FeatureFlags.ShowRegistration)]
public class ReprocessingInputsAndOutputsController(
	ISessionManager<ReprocessorRegistrationSession> sessionManager,
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

		var currentYear = DateTime.Now.Year;
		model.Year = currentYear == 2025 ? currentYear + 1 : currentYear;

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

			var currentYear = DateTime.Now.Year;
			model.Year = currentYear == 2025 ? currentYear + 1 : currentYear;

			return View(nameof(PackagingWasteWillReprocess), model);
		}

		reprocessingInputsOutputs.Materials
		.ForEach(p =>
			p.IsMaterialBeingAppliedFor = model.SelectedRegistrationMaterials
				.Contains(p.MaterialLookup.Name.ToString())
		);

		reprocessingInputsOutputs.CurrentMaterial = reprocessingInputsOutputs.Materials!.Find(m => m.IsMaterialBeingAppliedFor == true);

		await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

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
}