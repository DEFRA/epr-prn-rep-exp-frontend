using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterCheckYouAnswersForAddress)]
    public class CheckYourAnswersForNoticeAddressController : BaseExporterController<CheckYourAnswersForNoticeAddressController>
    {
        private const string ChangeValueKey = "ChangeValue";
        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/CheckYourAnswersForNoticeAddress/CheckYourAnswersForNoticeAddress.cshtml";
        private readonly IRegistrationService _service;

        public CheckYourAnswersForNoticeAddressController(
            ILogger<CheckYourAnswersForNoticeAddressController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper,
            IConfiguration configuration,
            IRegistrationService service) : base(logger, saveAndContinueService, sessionManager, mapper, configuration)
        {
            _service = service;
            base.NextPageInJourney = PagePaths.ExporterPlaceholder;
            base.CurrentPageInJourney = PagePaths.ExporterCheckYouAnswersForAddress;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrationId = await GetRegistrationIdAsync(Guid.NewGuid());

            await SetExplicitBackLink(PagePaths.ExporterPlaceholder, CurrentPageInJourney);
            var vm = new AddressViewModel();

            try
            {
                RegistrationDto dto = await _service.GetAsync(Session.RegistrationId.Value);
                if (dto != null) vm = Mapper.Map<AddressViewModel>(dto.LegalDocumentAddress);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to retrieve Address for registration {RegistrationId}", Session.RegistrationId);
            }

            return View(CurrentPageViewLocation, vm);
        }
    }
}
