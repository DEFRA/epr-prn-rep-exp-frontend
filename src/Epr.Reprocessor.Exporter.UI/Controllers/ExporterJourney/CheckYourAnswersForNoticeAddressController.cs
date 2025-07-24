using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterCheckYourAnswersForNotices)]
    public class CheckYourAnswersForNoticeAddressController : BaseExporterController<CheckYourAnswersForNoticeAddressController>
    {
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
            base.NextPageInJourney = PagePaths.ExporterRegistrationTaskList2;
            base.CurrentPageInJourney = PagePaths.ExporterCheckYouAnswersForAddress;
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrationId = Session.RegistrationId.Value;
            
            await SetExplicitBackLink(PagePaths.ExporterPlaceholder, CurrentPageInJourney);
            var vm = new CheckYourAnswersForNoticeAddressViewModel();

            if (Session.LegalAddress != null)
            {
                vm = Mapper.Map<CheckYourAnswersForNoticeAddressViewModel>(Session.LegalAddress);
            }
            else
            {
                var dto = await _service.GetByRegistrationId(registrationId);
                if (dto != null) {
                    vm = Mapper.Map<CheckYourAnswersForNoticeAddressViewModel>(dto);
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
            var vm = Mapper.Map<CheckYourAnswersForNoticeAddressViewModel>(Session.LegalAddress);
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
                case ConfirmAndContinueActionKey:
                    await SetStatusOfExporterJouney(Session.RegistrationId.Value, "AddressForNotices", ApplicantRegistrationTaskStatus.Completed);
                    return Redirect(PagePaths.ExporterRegistrationTaskList2);

                case SaveAndContinueLaterActionKey:
                    await SetStatusOfExporterJouney(Session.RegistrationId.Value, "AddressForNotices", ApplicantRegistrationTaskStatus.Started);
                    return ApplicationSaved();

                default:
                    return Redirect(PagePaths.ExporterPlaceholder);
            }
        }

        private async Task SetStatusOfExporterJouney(Guid registrationId, string taskName, ApplicantRegistrationTaskStatus taskStatus)
        {
            var dto = new UpdateRegistrationTaskStatusDto { Status = taskStatus.ToString(), TaskName = taskName };
            await _registrationService.UpdateRegistrationTaskStatusAsync(registrationId, dto);
        }
    }
}
