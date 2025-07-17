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
    private const string CurrentPage = PagePaths.ExporterAddressForNotice;
    private const string ViewPath = "~/Views/ExporterJourney/AddressForServiceOfNotice/AddressForNotices.cshtml";

    private IValidationService ValidationService { get; } = validationService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await InitialiseSession();
        var sessionReprocessingSite = Session.RegistrationApplicationSession.ReprocessingSite;

        Session.Journey =
        [
            sessionReprocessingSite!.ServiceOfNotice!.SourcePage ?? PagePaths.ExporterRegistrationTaskList,
            CurrentPage
        ];

        SetBackLink(CurrentPage);

        AddressForNoticesViewModel model = null;

        var userOrganisation = HttpContext.GetUserData().Organisations?.FirstOrDefault();

        if (userOrganisation != null)
        {
            model = new AddressForNoticesViewModel
            {
                SelectedAddressOptions = sessionReprocessingSite.TypeOfAddress,
                IsBusinessAddress = string.IsNullOrEmpty(userOrganisation.CompaniesHouseNumber),
                BusinessAddress = Mapper.Map<AddressViewModel>(userOrganisation),
                SiteAddress = Mapper.Map<AddressViewModel>(sessionReprocessingSite),
                ShowSiteAddress = sessionReprocessingSite.TypeOfAddress == AddressOptions.DifferentAddress
            };

            await SaveSession(CurrentPage);
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    [Route(PagePaths.AddressForNotices)]
    public async Task<IActionResult> Post(AddressForNoticesViewModel model)
    {
        await InitialiseSession();

        SetBackLink(CurrentPage);

        var validationResult = await ValidationService.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(ViewPath, model);
        }

        if (model.SelectedAddressOptions is not AddressOptions.DifferentAddress)
        {
            Session.LegalAddress = Mapper.Map<AddressDto>(model.GetAddress());
        }

        await SaveSession(CurrentPage);

        return Redirect(model.SelectedAddressOptions is AddressOptions.DifferentAddress 
            ? PagePaths.ExporterPostcodeForNotices
            : PagePaths.ExporterCheckYourAnswersForNotices);
    }
}
