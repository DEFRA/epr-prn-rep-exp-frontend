using Epr.Reprocessor.Exporter.UI.Mapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route("exporter")]
    [FeatureGate(FeatureFlags.ShowRegistration)]
    public class ExporterAddressForNoticesController : RegistrationControllerBase
    {
        private readonly ILogger<ExporterAddressForNoticesController> _logger;

        public ExporterAddressForNoticesController(
            ILogger<ExporterAddressForNoticesController> logger,
            ISessionManager<ReprocessorRegistrationSession> sessionManager,
            IReprocessorService reprocessorService,
            IPostcodeLookupService postcodeLookupService,
            IValidationService validationService,
            IStringLocalizer<SelectAuthorisationType> selectAuthorisationStringLocalizer,
            IRequestMapper requestMapper)
            : base(sessionManager, reprocessorService, postcodeLookupService, validationService, selectAuthorisationStringLocalizer, requestMapper)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route(PagePaths.ExporterAddressForNotices)]
        public async Task<IActionResult> AddressForNotices()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
            session.Journey = [reprocessingSite!.SourcePage, PagePaths.ExporterAddressForNotices];

            SetBackLink(session, PagePaths.ExporterAddressForNotices);

            var organisation = HttpContext.GetUserData().Organisations.FirstOrDefault()
                ?? throw new ArgumentNullException("organisation");

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

            await SaveSession(session, PagePaths.ExporterAddressForNotices);
            return View(nameof(AddressForNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.ExporterAddressForNotices)]
        public async Task<IActionResult> AddressForNotices(AddressForNoticesViewModel model, string buttonAction)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
            session.Journey = [reprocessingSite!.SourcePage, PagePaths.ExporterAddressForNotices];

            SetBackLink(session, PagePaths.ExporterAddressForNotices);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(nameof(AddressForNotices), model);
            }

            if (model.SelectedAddressOptions == AddressOptions.DifferentAddress)
            {
                reprocessingSite.ServiceOfNotice!.TypeOfAddress = AddressOptions.DifferentAddress;
                await SaveSession(session, PagePaths.ExporterAddressForNotices);
                return Redirect(PagePaths.ExporterPostcodeForNotices);
            }

            reprocessingSite.ServiceOfNotice!.SetAddress(model.GetAddress(), model.SelectedAddressOptions);
            await SaveSession(session, PagePaths.ExporterAddressForNotices);

            return Redirect(PagePaths.ExporterCheckYourAnswersForNotices);
        }

        [HttpGet]
        [Route(PagePaths.ExporterPostcodeForNotices)]
        public async Task<IActionResult> PostcodeForServiceOfNotices()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.ExporterAddressForNotices, PagePaths.ExporterPostcodeForNotices];

            SetBackLink(session, PagePaths.ExporterPostcodeForNotices);

            var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice?.LookupAddress;
            var model = new PostcodeForServiceOfNoticesViewModel(lookupAddress?.Postcode);

            await SaveSession(session, PagePaths.ExporterPostcodeForNotices);
            return View(nameof(PostcodeForServiceOfNotices), model);
        }

        [HttpPost]
        [Route(PagePaths.ExporterPostcodeForNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostcodeForServiceOfNotices(PostcodeForServiceOfNoticesViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session.Journey = [PagePaths.ExporterAddressForNotices, PagePaths.ExporterPostcodeForNotices];

            SetBackLink(session, PagePaths.ExporterPostcodeForNotices);

            var validationResult = await ValidationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(nameof(PostcodeForServiceOfNotices), model);
            }

            var lookup = session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice!.LookupAddress;
            lookup.Postcode = model.Postcode;

            var addresses = await PostcodeLookupService.GetAddressListByPostcodeAsync(lookup.Postcode);
            session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.LookupAddress = new LookupAddress(model.Postcode, addresses, lookup.SelectedAddressIndex);

            await SaveSession(session, PagePaths.ExporterPostcodeForNotices);
            return Redirect(PagePaths.ConfirmNoticesAddress);
        }

        [HttpGet]
        [Route(PagePaths.ConfirmNoticesAddress)]
        public async Task<IActionResult> ConfirmNoticesAddress()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var address = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.Address;

            var model = new ConfirmNoticesAddressViewModel
            {
                ConfirmAddress = address != null ? string.Join(", ",
                    new[] { address.AddressLine1, address.AddressLine2, address.Town, address.County, address.Postcode }
                    .Where(x => !string.IsNullOrWhiteSpace(x))) : string.Empty
            };

            await SaveSession(session, PagePaths.ConfirmNoticesAddress);
            return View(nameof(ConfirmNoticesAddress), model);
        }

        [HttpPost]
        [Route(PagePaths.ConfirmNoticesAddress)]
        public async Task<IActionResult> ConfirmNoticesAddress(ConfirmNoticesAddressViewModel model)
        {
            await SaveSession(await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession(), PagePaths.ConfirmNoticesAddress);
            return Redirect(PagePaths.ExporterCheckYourAnswersForNotices);
        }

        [HttpGet]
        [Route(PagePaths.ExporterCheckYourAnswersForNotices)]
        public async Task<IActionResult> CheckAnswers()
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            var model = new CheckAnswersViewModel(session.RegistrationApplicationSession.ReprocessingSite!);

            await SaveSession(session, PagePaths.ExporterCheckYourAnswersForNotices);
            return View(nameof(CheckAnswers), model);
        }

        [HttpPost]
        [Route(PagePaths.ExporterCheckYourAnswersForNotices)]
        public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            await SaveSession(session, PagePaths.ExporterCheckYourAnswersForNotices);

            await MarkTaskStatusAsCompleted(TaskType.SiteAndContactDetails);
            return Redirect(PagePaths.RegistrationLanding);
        }

        [ExcludeFromCodeCoverage]
        private async Task MarkTaskStatusAsCompleted(TaskType taskType)
        {
            var session = await SessionManager.GetSessionAsync(HttpContext.Session);
            if (session?.RegistrationId is not null)
            {
                var updateDto = new UpdateRegistrationTaskStatusDto
                {
                    TaskName = taskType.ToString(),
                    Status = nameof(TaskStatuses.Completed)
                };

                try
                {
                    await ReprocessorService.Registrations.UpdateRegistrationTaskStatusAsync(session.RegistrationId.Value, updateDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating task status");
                    throw;
                }
            }
        }
    }
}
