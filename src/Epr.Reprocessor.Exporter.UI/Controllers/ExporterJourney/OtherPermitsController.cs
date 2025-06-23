using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
		// TODO: fix previous page in journey value
		// TODO: how do we handle exceptions?
		// TODO: what is the [SaveAndContinueExporterPlaceholderKey] for?
        private const string PreviousPageInJourney = PagePaths.ExporterPlaceholder;
		private const string NextPageInJourney = PagePaths.ExporterPlaceholder;
		private const string CurrentPageInJourney = PagePaths.OtherPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

		private readonly IOtherPermitsService _otherPermitsService = otherPermitsService;

		[HttpGet]
        public async Task<IActionResult> Get(Guid registrationId)
        {
            // TODO: I think the registration id is in session at this point and should not be passed in
            // var registrationid = await GetRegistrationIdAsync();

            SetBackLink(CurrentPageInJourney);

            if (registrationId == Guid.Empty)
                registrationId = Session.RegistrationId.Value;

            var dto = await _otherPermitsService.GetByRegistrationId(registrationId);
            UptickListToNumberOfItems(dto.WasteExemptionReference, 5);
            var vm = dto == null ? new OtherPermitsViewModel { RegistrationId = registrationId} : Mapper.Map<OtherPermitsViewModel>(dto);

            return View("~/Views/ExporterJourney/OtherPermits/OtherPermits.cshtml", vm);
        }

        private static void UptickListToNumberOfItems(List<string> list, int maxCount)
        {
            for (int i = 0; i < maxCount - 1; i++)
            {
                if (list.Count < maxCount)
                {
                    list.Add("");
                }
            }
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

            // TODO: how does this work?
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
    }
}
