using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> SessionManager,
    IRequestMapper requestMapper,
    IRegistrationService registrationService,
    IRegistrationMaterialService registrationMaterialService) : Controller
{
    [HttpGet]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public async Task<IActionResult> Index()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);
        // session.Journey = [PagePaths.TaskList, PagePaths.WastePermitExemptions];

        if (session?.RegistrationId is null)
        {
            //throw;
        }

        if (session?.ExporterRegistrationApplicationSession.RegistrationMaterialId is null)
        {
            //throw;
        }

        var overseasReprocessorSiteViewModel = new OverseasReprocessorSiteViewModel();
        overseasReprocessorSiteViewModel.Countries = await registrationService.GetCountries();


        //TODO: Populate the model with session data if available

        return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", overseasReprocessorSiteViewModel);
    }

    [HttpPost]
    [Route(PagePaths.OverseasReprocessorDetails)]
    public IActionResult Index(OverseasReprocessorSiteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Registration/Exporter/OverseasReprocessorDetails.cshtml", model);
        }

        // TODO: Save to session or service

        return RedirectToAction("NextStep"); // Update this with actual next step
    }
}