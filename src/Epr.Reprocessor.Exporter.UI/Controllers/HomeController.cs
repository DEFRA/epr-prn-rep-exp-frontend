using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HomeSettings _homeSettings;
    public HomeController(ILogger<HomeController> logger, IOptions<HomeSettings> homeSettings)
    {
        _logger = logger;
        _homeSettings = homeSettings.Value;
    }

    public IActionResult Index()
    {
        var userData = User.GetUserData();
        //var organisation = userData.Organisations.First(x => x.OrganisationRole == OrganisationRoles.Producer);
        var viewModel = new HomeSettings
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            AddOrganisation = _homeSettings.AddOrganisation,
            ViewOrganisations = _homeSettings.ViewOrganisations,
            ApplyReprocessor = _homeSettings.ApplyReprocessor,
            ApplyExporter = _homeSettings.ApplyExporter,
            ViewApplications = _homeSettings.ViewApplications,

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
