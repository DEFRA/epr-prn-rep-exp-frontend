using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterCheckYourAnswersForNotices)]
    public class CheckYourAnswersForNoticeAddressController : BaseExporterController<CheckYourAnswersForNoticeAddressController>
    {
        private const string ChangeValueKey = "ChangeValue";
        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/CheckYourAnswersForNoticeAddress/CheckYourAnswersForNoticeAddress.cshtml";
        private readonly ICheckYourAnswersForNoticeAddressService _service;
        private readonly IRegistrationService _registrationService;

        public CheckYourAnswersForNoticeAddressController(
            ILogger<CheckYourAnswersForNoticeAddressController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IMapper mapper,
            ICheckYourAnswersForNoticeAddressService service,
            IRegistrationService registrationService) : base(logger, saveAndContinueService, sessionManager, mapper)
        {
            _service = service;
            base.NextPageInJourney = PagePaths.ExporterRegistrationTaskList;
            base.CurrentPageInJourney = PagePaths.ExporterCheckYouAnswersForAddress;
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrationId = await GetRegistrationIdAsync(Guid.NewGuid());

            await SetExplicitBackLink(PagePaths.ExporterPlaceholder, CurrentPageInJourney);
            var vm = new AddressViewModel();

            if (Session.LegalAddress != null)
            {
                vm = Mapper.Map<AddressViewModel>(Session.LegalAddress);
            }
            else
            {
                var dto = await _registrationService.GetAsync(registrationId);
                if (dto != null && dto.LegalDocumentAddress != null) {
                    vm = Mapper.Map<AddressViewModel>(dto.LegalDocumentAddress);
                }
                else
                {
                    Logger.LogError("Unable to retrieve Address for registration {RegistrationId}", Session.RegistrationId);
                }
            }

            return View(CurrentPageViewLocation, vm);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string buttonAction)
        {
            var vm = Mapper.Map<AddressViewModel>(Session.LegalAddress);
            try
            {
                _service.Save(Session.RegistrationId.Value, Session.LegalAddress);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to save Address for Notices");
                throw;
            }

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(WasteCarrierBrokerDealerController),
                nameof(Get), JsonConvert.SerializeObject(vm), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return Redirect(PagePaths.ExporterPlaceholder);

                case SaveAndComeBackLaterActionKey:
                    return ApplicationSaved();

                default:
                    return Redirect(PagePaths.ExporterPlaceholder);
            }
        }
    }
}
