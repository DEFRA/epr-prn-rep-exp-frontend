using Epr.Reprocessor.Exporter.UI.Mapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController(
    ISessionManager<ExporterRegistrationSession> SessionManager,
    IRequestMapper requestMapper,
    IRegistrationService registrationService,
    IRegistrationMaterialService registrationMaterialService) : Controller
{


}