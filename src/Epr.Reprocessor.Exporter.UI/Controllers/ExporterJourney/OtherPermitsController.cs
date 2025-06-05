using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Constants;
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
    public class OtherPermitsController : BaseExporterController<OtherPermitsController>
    {
		// TODO: fix previous page in journey value
		// TODO: how do we handle exceptions?
		// TODO: what is the [SaveAndContinueExporterPlaceholderKey] for?
        private const string PreviousPageInJourney = PagePaths.ExporterPlaceholder;
		private const string NextPageInJourney = PagePaths.ExporterPlaceholder;
		private const string CurrentPageInJourney = PagePaths.OtherPermits;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

		private readonly IOtherPermitsService _otherPermitsService;

		public OtherPermitsController(
            ILogger<OtherPermitsController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ReprocessorExporterRegistrationSession> sessionManager,
            IMapper mapper,
            IOtherPermitsService otherPermitsService) : base(logger, saveAndContinueService, sessionManager, mapper)
        {         
            _otherPermitsService = otherPermitsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int registrationId)
        {
            SetBackLink(CurrentPageInJourney);
            
            var dto = await _otherPermitsService.GetByRegistrationId(registrationId);
            var vm = dto == null ? new OtherPermitsViewModel { RegistrationId = registrationId} : Mapper.Map<OtherPermitsViewModel>(dto);

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Save(OtherPermitsViewModel viewModel, string buttonAction)
        {
            if (!ModelState.IsValid)
            {
                return View("OtherPermits", viewModel);
            }

            try
            {
				var dto = Mapper.Map<OtherPermitsDto>(viewModel);
				_otherPermitsService.Save(dto);
			}
			catch (Exception)
            {
                throw;
            }

            // TODO: how does this work?
            SetBackLink(CurrentPageInJourney);
            await SaveSession(CurrentPageInJourney, NextPageInJourney);
            await SaveAndContinue(0, nameof(Get), nameof(ExporterPlaceholder), SaveAndContinueAreas.ExporterRegistration, 
                JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

            switch (buttonAction)
            {
                case SaveAndContinueActionKey:
                    return Redirect(PagePaths.Placeholder);

                case SaveAndComeBackLaterActionKey:
                    return Redirect(PagePaths.ApplicationSaved);

                default:
                    return View(nameof(OtherPermitsController));
            }
        }
    }
}
