using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

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
		private const string NextPageInJourney = PagePaths.ExporterPlaceholder;
		private const string CurrentPageInJourney = PagePaths.OtherPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

		private readonly IOtherPermitsService _otherPermitsService = otherPermitsService;

		[HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrationId = await GetRegistrationIdAsync(null);

            SetBackLink(CurrentPageInJourney);

            var dto = await _otherPermitsService.GetByRegistrationId(registrationId);
            var vm = dto == null
                ? new OtherPermitsViewModel { RegistrationId = registrationId }
                : Mapper.Map<OtherPermitsViewModel>(dto);

            if (dto != null)
            {
                UpsizeListToNumberOfItems(dto.WasteExemptionReference, 5);
            }

            return View("~/Views/ExporterJourney/OtherPermits/OtherPermits.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OtherPermitsViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View("OtherPermits", viewModel);
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

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(ExporterPlaceholder),
                nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return Redirect(PagePaths.ExporterPlaceholder);

                case SaveAndComeBackLaterActionKey:
                    return ApplicationSaved();

                default:
                    return View(nameof(OtherPermitsController));
            }
        }

        [HttpGet]
        [Route(PagePaths.CheckAnswers)]
        public async Task<IActionResult> CheckYourAnswers(Guid? registrationId)
        {
            var dto = await _otherPermitsService.GetByRegistrationId((Guid)registrationId);
            var vm = dto == null ? new OtherPermitsViewModel { RegistrationId = (Guid)registrationId } : Mapper.Map<OtherPermitsViewModel>(dto);

            return View(vm);
        }

        private static void UpsizeListToNumberOfItems(List<string> list, int maxCount)
        {
            for (int i = 0; i < maxCount - 1; i++)
            {
                if (list.Count < maxCount)
                {
                    list.Add("");
                }
            }
        }
    }
}
