using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LinkSettings _linkSettings;
    public HomeController(ILogger<HomeController> logger, IOptions<LinkSettings> linkSettings)
    {
        _logger = logger;
        _linkSettings = linkSettings.Value;
    }

    public IActionResult Index()
    {
        var userData = User.GetUserData();
        //var organisation = userData.Organisations.First(x => x.OrganisationRole == OrganisationRoles.Producer);
        var viewModel = new LinkSettings
        {
            AddOrganisation = _linkSettings.AddOrganisation,
            ViewOrganisations = _linkSettings.ViewOrganisations,
            ApplyReprocessor = _linkSettings.ApplyReprocessor,
            ApplyExporter = _linkSettings.ApplyExporter,
            ViewApplications = _linkSettings.ViewApplications
        };
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
