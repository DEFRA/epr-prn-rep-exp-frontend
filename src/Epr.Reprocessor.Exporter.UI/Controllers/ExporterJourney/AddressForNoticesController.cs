using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;

[Route(PagePaths.ExporterAddressForNotice)]
public class AddressForNoticesController(
    ILogger<AddressForNoticesController> logger,
    ISaveAndContinueService saveAndContinueService,
    ISessionManager<ExporterRegistrationSession> sessionManager,
    IValidationService validationService,
    IMapper mapper
    ) : BaseExporterController<AddressForNoticesController>(logger, saveAndContinueService, sessionManager, mapper)
{
    private const string CurrentPage = PagePaths.AddressForNotices;
    private const string ViewPath = "~/Views/ExporterJourney/AddressForServiceOfNotice/AddressForNotices.cshtml";

    private IValidationService ValidationService { get; } = validationService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await InitialiseSession();
        var session = Session;
        var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
        session.Journey =
        [
            reprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.ExporterRegistrationTaskList,
            CurrentPage
        ];

        SetBackLink(CurrentPage);

        var organisation = HttpContext.GetUserData().Organisations.FirstOrDefault();

        var model = new AddressForNoticesViewModel
        {
            SelectedAddressOptions = reprocessingSite.TypeOfAddress,
            IsBusinessAddress = string.IsNullOrEmpty(organisation.CompaniesHouseNumber),
            BusinessAddress = new AddressViewModel
            {
                AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                AddressLine2 = organisation.Locality,
                TownOrCity = organisation.Town ?? string.Empty,
                County = organisation.County ?? string.Empty,
                Postcode = organisation.Postcode ?? string.Empty
            },
            SiteAddress = new AddressViewModel
            {
                AddressLine1 = reprocessingSite.Address?.AddressLine1 ?? string.Empty,
                AddressLine2 = reprocessingSite.Address?.AddressLine2 ?? string.Empty,
                TownOrCity = reprocessingSite.Address?.Town ?? string.Empty,
                County = reprocessingSite.Address?.County ?? string.Empty,
                Postcode = reprocessingSite.Address?.Postcode ?? string.Empty
            },
            ShowSiteAddress = reprocessingSite.TypeOfAddress == AddressOptions.DifferentAddress
        };

        await SaveSession(CurrentPage);
        return View(ViewPath, model);
    }

    [HttpPost]
    [Route(PagePaths.AddressForNotices)]
    public async Task<IActionResult> Post(AddressForNoticesViewModel model)
    {
        await InitialiseSession();
        var session = Session;
        var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

        session.Journey =
        [
            reprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.ExporterRegistrationTaskList,
            CurrentPage
        ];

        SetBackLink(CurrentPage);

        var validationResult = await ValidationService.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        if (model.SelectedAddressOptions is AddressOptions.DifferentAddress)
        {
            reprocessingSite.ServiceOfNotice!.TypeOfAddress = AddressOptions.DifferentAddress;
        }
        else
        {
            reprocessingSite.ServiceOfNotice!.SetAddress(model.GetAddress(), model.SelectedAddressOptions);
        }

        await SaveSession(CurrentPage);

        return Redirect(model.SelectedAddressOptions is AddressOptions.DifferentAddress ? PagePaths.ExporterPostcodeForServiceOfNotices : PagePaths.ExporterCheckYourAnswersForNotices);
    }
}
