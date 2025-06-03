using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LinksConfig _linksConfig;
    public HomeController(ILogger<HomeController> logger, IOptions<LinksConfig> linksConfig)
    {
        _logger = logger;
        _linksConfig = linksConfig.Value;
    }

    public IActionResult Index()
    {
        var userData = User.GetUserData();
        var viewModel = new HomeViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            AddOrganisation = _linksConfig.AddOrganisation,
            ViewOrganisations = _linksConfig.ViewOrganisations,
            ApplyReprocessor = _linksConfig.ApplyReprocessor,
            ApplyExporter = _linksConfig.ApplyExporter,
            ViewApplications = _linksConfig.ViewApplications,

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
