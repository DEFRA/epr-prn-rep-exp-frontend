using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[ExcludeFromCodeCoverage]
[Route(PagePaths.TeamLanding)]
public class TeamController : Controller
{
    private readonly ILogger<TeamController> _logger;

    public TeamController(ILogger<TeamController> logger)
    {
        _logger = logger;
    }
}