using AutoMapper;
using static Epr.Reprocessor.Exporter.UI.App.Constants.Endpoints;

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
        var sessionReprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
        session.Journey =
        [
            sessionReprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.ExporterRegistrationTaskList,
            CurrentPage
        ];

        SetBackLink(CurrentPage);

        AddressForNoticesViewModel model = null;

        try
        {
            var userOrganisation = HttpContext.GetUserData().Organisations.FirstOrDefault();

            if (userOrganisation != null)
            {
                model = new AddressForNoticesViewModel
                {
                    SelectedAddressOptions = sessionReprocessingSite.TypeOfAddress,
                    IsBusinessAddress = string.IsNullOrEmpty(userOrganisation.CompaniesHouseNumber),
                    BusinessAddress = new AddressViewModel
                    {
                        AddressLine1 = $"{userOrganisation.BuildingNumber} {userOrganisation.Street}",
                        AddressLine2 = userOrganisation.Locality,
                        TownOrCity = userOrganisation.Town ?? string.Empty,
                        County = userOrganisation.County ?? string.Empty,
                        Postcode = userOrganisation.Postcode ?? string.Empty
                    },
                    SiteAddress = new AddressViewModel
                    {
                        AddressLine1 = sessionReprocessingSite.Address?.AddressLine1 ?? string.Empty,
                        AddressLine2 = sessionReprocessingSite.Address?.AddressLine2 ?? string.Empty,
                        TownOrCity = sessionReprocessingSite.Address?.Town ?? string.Empty,
                        County = sessionReprocessingSite.Address?.County ?? string.Empty,
                        Postcode = sessionReprocessingSite.Address?.Postcode ?? string.Empty
                    },
                    ShowSiteAddress = sessionReprocessingSite.TypeOfAddress == AddressOptions.DifferentAddress
                };

                await SaveSession(CurrentPage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to retrieve organisation details for user");
        }

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

    private
}
