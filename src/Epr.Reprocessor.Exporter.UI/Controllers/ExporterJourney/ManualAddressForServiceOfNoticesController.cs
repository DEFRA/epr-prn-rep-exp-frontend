using AutoMapper;
using Epr.Reprocessor.Exporter.UI.Controllers;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;

[Route(PagePaths.ExporterManualAddressForServiceOfNotices)]
[FeatureGate(FeatureFlags.ShowRegistration)]
public class ManualAddressForServiceOfNoticesController(
    ILogger<ManualAddressForServiceOfNoticesController> logger,
    ISaveAndContinueService saveAndContinueService,
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IValidationService validationService,
    IMapper mapper)
    : BaseExporterController<ManualAddressForServiceOfNoticesController>(logger, saveAndContinueService, sessionManager, mapper)
{
    private const string CurrentPage = PagePaths.ExporterManualAddressForServiceOfNotices;
    private const string NextPage = PagePaths.ExporterCheckYourAnswersForNotices;
    private const string ViewPath = "~/Views/ExporterJourney/AddressForServiceOfNotice/ManualAddressForNotices.cshtml";
    private const string SaveAndContinueExporterPlaceholderKey = "ExporterManualAddressForNoticeSaveAndContinueKey";

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await InitialiseSession();
        SetBackLink(CurrentPage);

        var model = new ManualAddressForServiceOfNoticesViewModel();
        var address = Session.LegalAddress;

        if (address is not null)
        {
            model = new ManualAddressForServiceOfNoticesViewModel
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                County = address.County,
                Postcode = address.PostCode,
                TownOrCity = address.TownCity
            };
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(ManualAddressForServiceOfNoticesViewModel model, string buttonAction)
    {
        await InitialiseSession();
        SetBackLink(CurrentPage);

        var validationResult = await validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(ViewPath, model);
        }

        Session.LegalAddress = new AddressDto
        {
            AddressLine1 = model.AddressLine1,
            AddressLine2 = model.AddressLine2,
            TownCity = model.TownOrCity,
            County = model.County,
            PostCode = model.Postcode,
            NationId = 1,
            GridReference = string.Empty
        };

        await PersistJourneyAndSession(CurrentPage, NextPage, SaveAndContinueAreas.ExporterRegistration,
            nameof(ManualAddressForServiceOfNoticesController), nameof(Get),
            JsonConvert.SerializeObject(model), SaveAndContinueExporterPlaceholderKey);

        return buttonAction switch
        {
            SaveAndContinueActionKey => Redirect(NextPage),
            ConfirmAndContinueActionKey => Redirect(NextPage),
            SaveAndComeBackLaterActionKey => Redirect(PagePaths.ExporterPlaceholder),
            _ => View(ViewPath, model)
        };
    }
}
