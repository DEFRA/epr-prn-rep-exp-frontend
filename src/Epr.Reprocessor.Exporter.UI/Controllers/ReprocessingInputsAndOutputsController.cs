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

		await SaveSession(session, PagePaths.PackagingWasteWillReprocess);

		if (buttonAction is SaveAndContinueActionKey)
		{
			if (model.SelectedRegistrationMaterials.Count == reprocessingInputsOutputs.Materials.Count)
			{
				reprocessingInputsOutputs.CurrentMaterial = reprocessingInputsOutputs.Materials!.Find(m => m.IsMaterialSelected == true);
				return Redirect(PagePaths.ReprocessingOutputsForLastCalendarYear);
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
    [Route(PagePaths.ReprocessingOutputsForLastCalendarYear)]
    public async Task<IActionResult> ReprocessingOutputsForLastYear()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.ReprocessIngInputsForLastCalendarYear, PagePaths.ReprocessingOutputsForLastCalendarYear];

        // TODO: this needs uncommenting when the data is available in session.
        //if (session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial is null)
        //{
        //    return Redirect(PagePaths.TaskList);
        //}
        var material = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;
        await SaveSession(session, PagePaths.ReprocessingOutputsForLastCalendarYear);
        SetBackLink(session, PagePaths.ReprocessingOutputsForLastCalendarYear);

        
        await SaveSession(session, PagePaths.ReprocessingOutputsForLastCalendarYear);

		// This is just test data until we have this available in session. TODO: remove this once you have it in session. 
        var materialoutput = new MaterialOutputSummaryModel()
        {
            MaterialName = material?.MaterialLookup?.Name.ToString()??"Steel",
            TotalInputTonnes = 100,
            ReprocessedMaterials = new List<ReprocessedMaterial>()

        };
        for (int i = 0; i < 10; i++)
        {
            materialoutput.ReprocessedMaterials.Add(new ReprocessedMaterial());
        }
        
        return View(nameof(ReprocessingOutputsForLastYear), materialoutput);

    }
    [HttpPost]
    [Route(PagePaths.ReprocessingOutputsForLastCalendarYear)]
    public async Task<IActionResult> ReprocessingOutputsForLastCalendarYear(MaterialOutputSummaryModel model, string buttonAction)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [PagePaths.TaskList, PagePaths.PackagingWasteWillReprocess];

        SetBackLink(session, PagePaths.ReprocessingOutputsForLastCalendarYear);

        var reprocessingOutputs = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.ReprocessingOutput;
        var material = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial;
        if (!ModelState.IsValid)
        {           
            return View(nameof(ReprocessingOutputsForLastCalendarYear), model);
        }
        //session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Id,
        reprocessingOutputs.RegistrationReprocessingIOId = new Guid("7E45AD40-3322-47A1-9E84-6EB96480CB39");
        reprocessingOutputs.RegistrationMaterialId = material.Id;
        reprocessingOutputs.SentToOtherSiteTonnes = model.SentToOtherSiteTonnes;
		reprocessingOutputs.ContaminantTonnes = model.ContaminantTonnes;
        reprocessingOutputs.ProcessLossTonnes = model.ProcessLossTonnes;
        reprocessingOutputs.RawMaterialorProduct = model.ReprocessedMaterials
            .Select(rm => new ReprocessingIORawMaterialorProductDto
            {
                ReprocessedTonnes = rm.ReprocessedTonnes,
                RawMaterialNameorProductName = rm.MaterialOrProduct
            }).ToList();

        await SaveSession(session, PagePaths.ReprocessingOutputsForLastCalendarYear);

        if (buttonAction is SaveAndContinueActionKey)
        {
             return Redirect(PagePaths.ReasonNotReprocessing);
        }

        if (buttonAction is SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }
}