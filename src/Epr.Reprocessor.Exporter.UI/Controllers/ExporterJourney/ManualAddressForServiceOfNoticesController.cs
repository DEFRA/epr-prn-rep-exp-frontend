using AutoMapper;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [Route(PagePaths.ExporterManualAddressForServiceOfNotices)]
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
                model = Mapper.Map<ManualAddressForServiceOfNoticesViewModel>(address);
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

            var address = Mapper.Map<AddressDto>(model);
            address.NationId = 1;
            address.GridReference = string.Empty;
            Session.LegalAddress = address;

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

}