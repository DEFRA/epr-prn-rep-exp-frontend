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

		session.RegistrationId = Guid.Parse("3B90C092-C10E-450A-92AE-F3DF455D2D95");//I will delete this line

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

		if (model.SelectedRegistrationMaterials.Count > 0)
		{
			reprocessingInputsOutputs.Materials
				.Where(m => model.SelectedRegistrationMaterials.Contains(m.MaterialLookup.Name.ToString())).ToList()
				.ForEach(p => p.IsMaterialSelected = true);
		}

        reprocessingInputsOutputs.CurrentMaterial = reprocessingInputsOutputs.Materials!.Find(m => m.IsMaterialSelected == true);

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

        session.Journey = [PagePaths.PackagingWasteWillReprocess, PagePaths.ApplicationContactName];

        var userData = User.GetUserData();
        var organisationPersons = await GetOrganisationPersons(userData);

        var viewModel = new ApplicationContactNameViewModel();
        viewModel.MapForView(userData.Id, currentMaterial, organisationPersons);

        SetBackLink(session, PagePaths.ApplicationContactName);
        await SaveSession(session, PagePaths.ApplicationContactName);

        return View(nameof(ApplicationContactName), viewModel);
    }

    [HttpPost]
    [Route(PagePaths.ApplicationContactName)]
    public async Task<IActionResult> ApplicationContactName(ApplicationContactNameViewModel viewModel, string buttonAction)
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
        
        currentMaterial.RegistrationMaterialContact = await registrationMaterialService.UpsertRegistrationMaterialContactAsync(
            currentMaterial.Id, currentMaterial.RegistrationMaterialContact);

        await SaveSession(session, PagePaths.ApplicationContactName);

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return RedirectToAction("TypeOfSuppliers", "ReprocessingInputsAndOutputs");
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
}