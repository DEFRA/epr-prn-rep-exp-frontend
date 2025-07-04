using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
	[Route(PagePaths.ExporterCheckYourAnswersPermits)]
    public class CheckYourAnswersPermitsController(
			ILogger<CheckYourAnswersPermitsController> logger,
			ISaveAndContinueService saveAndContinueService,
			ISessionManager<ExporterRegistrationSession> sessionManager,
			IMapper mapper,
            IConfiguration configuration,
            IOtherPermitsService otherPermitsService) : BaseExporterController<CheckYourAnswersPermitsController>(logger, saveAndContinueService, sessionManager, mapper, configuration)
    {
		private const string NextPageInJourney = PagePaths.ExporterPlaceholder;
		private const string CurrentPageInJourney = PagePaths.ExporterCheckYourAnswersPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

		private const string CurrentPageViewLocation = "~/Views/ExporterJourney/CheckYourAnswersPermits/CheckYourAnswersPermits.cshtml";

        private readonly IOtherPermitsService _otherPermitsService = otherPermitsService;

        private OtherPermitsViewModel InitializeViewModel(OtherPermitsDto dto, Guid registrationId)
        {
            var vm = dto != null
                ? Mapper.Map<OtherPermitsViewModel>(dto)
                : new OtherPermitsViewModel { RegistrationId = registrationId };

            if (vm.WasteExemptionReference == null)
                vm.WasteExemptionReference = new List<string>();

            if (vm.WasteExemptionReference.Count != 0)
                vm.WasteExemptionReference.Add(string.Empty);

            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrationId = await GetRegistrationIdAsync(null);

            SetBackLink(PagePaths.ExporterCheckYourAnswersPermits);

            OtherPermitsViewModel vm = null;
            try
            {
                var dto = await _otherPermitsService.GetByRegistrationId(registrationId);
                vm = InitializeViewModel(dto, registrationId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to retrieve Other Permits for registration {RegistrationId}", registrationId);
                vm = InitializeViewModel(null, registrationId);
            }

            return View(CurrentPageViewLocation, vm);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OtherPermitsViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View(CurrentPageViewLocation, viewModel);
            }

            try
            {
                viewModel.WasteExemptionReference = viewModel.WasteExemptionReference.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                var dto = Mapper.Map<OtherPermitsDto>(viewModel);
                _otherPermitsService.Save(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to save Other Permits");
                throw;
            }

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(CheckYourAnswersPermitsController),
                nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case ConfirmAndContinueActionKey:
                    return Redirect(PagePaths.ExporterPlaceholder);

                case SaveAndContinueLaterActionKey:
                    return ApplicationSaved();

                default:
                    return View(nameof(CheckYourAnswersPermitsController));
            }
        }
    }
}
