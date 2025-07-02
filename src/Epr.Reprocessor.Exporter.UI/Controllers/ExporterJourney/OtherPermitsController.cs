using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Humanizer;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
	[Route(PagePaths.OtherPermits)]
    public class OtherPermitsController(
			ILogger<OtherPermitsController> logger,
			ISaveAndContinueService saveAndContinueService,
			ISessionManager<ExporterRegistrationSession> sessionManager,
			IMapper mapper,
			IOtherPermitsService otherPermitsService) : BaseExporterController<OtherPermitsController>(logger, saveAndContinueService, sessionManager, mapper)
    {
		private const string NextPageInJourney = PagePaths.ExporterCheckYourAnswers;
		private const string CurrentPageInJourney = PagePaths.OtherPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

		private const string CurrentPageViewLocation = "~/Views/ExporterJourney/OtherPermits/OtherPermits.cshtml";
        private const string CheckYourAnswersPageViewLocation = "~/Views/ExporterJourney/OtherPermits/CheckYourAnswers.cshtml";

        private readonly IOtherPermitsService _otherPermitsService = otherPermitsService;


		[HttpGet]
        public async Task<IActionResult> Get(Guid? registrationId = null)
        {
			registrationId = await GetRegistrationIdAsync(registrationId);

			SetBackLink(CurrentPageInJourney);

            OtherPermitsViewModel vm = null;

            try
            {
                var dto = await _otherPermitsService.GetByRegistrationId(registrationId.Value);
                if (dto != null)
                {
                    UpsizeListToNumberOfItems(dto.WasteExemptionReference, 5);
                    vm = Mapper.Map<OtherPermitsViewModel>(dto);
                    vm.WasteExemptionReference = dto.WasteExemptionReference;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to retrieve Other Permits for registration {RegistrationId}", registrationId.Value);
            }
            finally
            {
                if (vm == null)
                {
                    vm = new OtherPermitsViewModel { RegistrationId = registrationId.Value };
                    vm.WasteExemptionReference = new List<string>();
                    UpsizeListToNumberOfItems(vm.WasteExemptionReference, 5);
                }
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

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(OtherPermitsController),
                nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return RedirectToAction(PagePaths.ExporterCheckYourAnswers);

				case SaveAndComeBackLaterActionKey:
                    return ApplicationSaved();

                case ConfirmAndContinueActionKey:
                    return Redirect(PagePaths.ExporterPlaceholder);

                case SaveAndContinueLaterActionKey:
                    return Redirect(PagePaths.ExporterPlaceholder);

                default:
                    return View(nameof(OtherPermitsController));
            }
        }

        [HttpGet]
        [Route(PagePaths.ExporterCheckYourAnswers)]
        [ActionName(PagePaths.ExporterCheckYourAnswers)]
        public async Task<IActionResult> CheckYourAnswers(Guid? registrationId)
        {
			registrationId = await GetRegistrationIdAsync(null);

            SetBackLink(PagePaths.ExporterCheckYourAnswers);

            OtherPermitsViewModel vm = null;
            try
            {
                var dto = await _otherPermitsService.GetByRegistrationId(registrationId.Value);
                if (dto != null)
                {
                    vm = Mapper.Map<OtherPermitsViewModel>(dto);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to retrieve Other Permits for registration {RegistrationId}", registrationId);
            }
            finally
            {
                if (vm == null)
                {
                    vm = new OtherPermitsViewModel { RegistrationId = registrationId.Value };
                }
            }

            return View(CheckYourAnswersPageViewLocation, vm);
		}

        private static void UpsizeListToNumberOfItems(List<string> list, int maxCount)
        {
            if (list == null) return;
            while (list.Count < maxCount)
            {
                list.Add(string.Empty);
            }
        }
    }
}
