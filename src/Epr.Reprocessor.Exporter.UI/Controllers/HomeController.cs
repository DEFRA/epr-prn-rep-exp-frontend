using Epr.Reprocessor.Exporter.UI.App.Options;
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
    private readonly FrontEndAccountCreationOptions _frontEndAccountCreation;
    private readonly ExternalUrlOptions _externalUrlOptions;

    public static class RouteIds
    {
        public const string ManageOrganisation = "home.manage-organisation";
        public const string AddOrganisation = "home.add-organisation-details";
    }

    public HomeController(ILogger<HomeController> logger,
        IOptions<LinksConfig> linksConfig,
        IOptions<FrontEndAccountCreationOptions> frontendAccountCreation,
        IOptions<ExternalUrlOptions> externalUrlOptions)
    {
        _logger = logger;
        _linksConfig = linksConfig.Value;
        _frontEndAccountCreation = frontendAccountCreation.Value;
        _externalUrlOptions = externalUrlOptions.Value;
    }

    public IActionResult Index()
    {
        var userData = User.GetUserData();
        
        if (User.GetOrganisationId() == null)
            return RedirectToAction(nameof(AddOrganisation));
        
        if (userData.Organisations.Count > 1)
            return RedirectToAction(nameof(SelectOrganisation));

        return RedirectToAction(nameof(ManageOrganisation));
    }

    [HttpGet]
    [Route(PagePaths.AddOrganisation, Name = RouteIds.AddOrganisation)]
    public IActionResult AddOrganisation()
    {
        var userData = User.GetUserData();

        if (User.GetOrganisationId() != null)
        {
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new AddOrganisationViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            AddOrganisationLink = _frontEndAccountCreation.AddOrganisation,
            ReadMoreAboutApprovedPersonLink = _externalUrlOptions.ReadMoreAboutApprovedPerson
        };

        return View(viewModel);
    }
    
    [HttpGet]
    [Route(PagePaths.ManageOrganisation, Name = RouteIds.ManageOrganisation)]
    public IActionResult ManageOrganisation()
    {
        var userData = User.GetUserData();

        if (User.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var organisation = userData.Organisations[0];

        var viewModel = new HomeViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            OrganisationName = organisation.Name,
            OrganisationNumber = organisation.OrganisationNumber,
            ApplyForRegistration = _linksConfig.ApplyForRegistration,
            ViewApplications = _linksConfig.ViewApplications,

        };
        
        return View(viewModel);
    }
    
    [HttpGet]
    [Route(PagePaths.SelectOrganisation)]
    public IActionResult SelectOrganisation()
    {
        var userData = User.GetUserData();

        var viewModel = new SelectOrganisationViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            Organisations = userData.Organisations.Select(org => new OrganisationViewModel
            {
                OrganisationName = org.Name,
                OrganisationNumber = org.OrganisationNumber
            }).ToList()
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
