using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[ExcludeFromCodeCoverage]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReprocessorService _reprocessorService;
    private readonly ISessionManager<ReprocessorRegistrationSession> _sessionManager;
    private readonly IOrganisationAccessor _organisationAccessor;
    private readonly LinksConfig _linksConfig;

    public static class RouteIds
    {
        public const string ManageOrganisation = "home.manage-organisation";
    }
    
    public HomeController(
        ILogger<HomeController> logger,
        IOptions<LinksConfig> linksConfig,
        IReprocessorService reprocessorService,
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        IOrganisationAccessor organisationAccessor)
    {
        _logger = logger;
        _reprocessorService = reprocessorService;
        _sessionManager = sessionManager;
        _organisationAccessor = organisationAccessor;
        _linksConfig = linksConfig.Value;
    }

    public async Task<IActionResult> Index()
    {
        var user = _organisationAccessor.OrganisationUser;

        if (user?.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(AddOrganisation));
        }

        var existingRegistration = await _reprocessorService.Registrations.GetByOrganisationAsync(
            (int)ApplicationType.Reprocessor,
            user.GetOrganisationId()!.Value);


        var materials =
            await _reprocessorService.RegistrationMaterials.GetAllRegistrationMaterialsAsync(
                Guid.Parse("98f11e62-9441-4266-8f4e-9fafa5a7fd26"));

        if (existingRegistration is not null)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            session!.SetFromExisting(existingRegistration);
            await _sessionManager.SaveSessionAsync(HttpContext.Session, session);
        }

        if (_organisationAccessor.Organisations.Count > 1)
        {
            return RedirectToAction(nameof(SelectOrganisation));
        }

        return RedirectToAction(nameof(ManageOrganisation));
    }

    [ExcludeFromCodeCoverage(Justification ="Logic for this is going to be defined on future sprint")]
    [HttpGet]
    [Route(PagePaths.AddOrganisation)]
    public IActionResult AddOrganisation()
    {
        return Ok("This is place holder for add organisation logic which need new view saying you don't have any org add new org and still on discussion");
    }
    
    [HttpGet]
    [Route(PagePaths.ManageOrganisation, Name = RouteIds.ManageOrganisation)]
    public IActionResult ManageOrganisation()
    {
        var user = _organisationAccessor.OrganisationUser!;
        var userData = user.GetUserData();
        var organisation = user.GetUserData().Organisations[0];

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
        var user = _organisationAccessor.OrganisationUser!;
        var userData = user.GetUserData();

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