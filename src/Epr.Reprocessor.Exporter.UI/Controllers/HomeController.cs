using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReprocessorService _reprocessorService;
    private readonly ISessionManager<ReprocessorRegistrationSession> _sessionManager;
    private readonly IOrganisationAccessor _organisationAccessor;
    private readonly LinksConfig _linksConfig;
    private readonly FrontEndAccountCreationOptions _frontEndAccountCreation;
    private readonly ExternalUrlOptions _externalUrlOptions;

    public static class RouteIds
    {
        public const string ManageOrganisation = "home.manage-organisation";
    }

    public HomeController(
        ILogger<HomeController> logger,
        IOptions<LinksConfig> linksConfig,
        IReprocessorService reprocessorService,
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        IOrganisationAccessor organisationAccessor,
        IOptions<FrontEndAccountCreationOptions> frontendAccountCreation,
        IOptions<ExternalUrlOptions> externalUrlOptions
        )
    {
        _logger = logger;
        _reprocessorService = reprocessorService;
        _sessionManager = sessionManager;
        _organisationAccessor = organisationAccessor;
        _linksConfig = linksConfig.Value;
        _frontEndAccountCreation = frontendAccountCreation.Value;
        _externalUrlOptions = externalUrlOptions.Value;
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

    [HttpGet]
    [Route(PagePaths.AddOrganisation)]
    public IActionResult AddOrganisation()
    {
        var user = _organisationAccessor.OrganisationUser;

        if (user!.GetOrganisationId() != null)
        {
            return RedirectToAction(nameof(Index));
        }

        var userData = user!.GetUserData();

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
    public async Task<IActionResult> ManageOrganisation()
    {
        var user = _organisationAccessor.OrganisationUser!;
        if (User.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(Index));
        }

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
            RegistrationData = await GetRegistrationDataAsync((Guid)organisation.Id),
            AccreditationData = await GetAccreditationDataAsync(organisation.Id)
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

    private async Task<List<RegistrationDataViewModel>> GetRegistrationDataAsync(Guid organisationId)
    {
        var registrations = await _reprocessorService.Registrations.GetRegistrationAndAccreditationAsync(organisationId);

        return registrations.Select(r =>
        {
            return new RegistrationDataViewModel
            {
                Material = r.Material,
                ApplicationType = r.ApplicationTypeId,
                SiteAddress = $"{r.ReprocessingSiteAddress?.AddressLine1}, {r.ReprocessingSiteAddress?.TownCity}",
                RegistrationStatus = (RegistrationStatus)r.RegistrationStatus,
                Year = r.Year,
                RegistrationContinueLink = _linksConfig.RegistrationContinueLink
            };
        }).ToList();
    }

    private async Task<List<AccreditationDataViewModel>> GetAccreditationDataAsync(Guid? organisationId)
    {
        var accreditations = await _reprocessorService.Registrations.GetRegistrationAndAccreditationAsync((Guid)organisationId);

        return accreditations.Select(r =>
        {
            return new AccreditationDataViewModel
            {
                Material = r.Material,
                ApplicationType = r.ApplicationTypeId,
                SiteAddress = $"{r.ReprocessingSiteAddress?.AddressLine1},{r.ReprocessingSiteAddress?.TownCity}",
                AccreditationStatus = (Enums.AccreditationStatus)r.AccreditationStatus,
                Year = r.Year,
                AccreditationContinueLink = _linksConfig.AccreditationContinueLink,
                AccreditationStartLink = _linksConfig.AccreditationStartLink
            };
        }).ToList();
    }
}