using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.ViewModels.Team;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[SuppressMessage("Major Code Smell", "S107:HomeController Methods should not have too many parameters", Justification = "Its Allowed for now in this case")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReprocessorService _reprocessorService;
    private readonly ISessionManager<ReprocessorRegistrationSession> _sessionManager;
    private readonly IOrganisationAccessor _organisationAccessor;
    private readonly LinksConfig _linksConfig;
    private readonly FrontEndAccountCreationOptions _frontEndAccountCreation;
    private readonly ExternalUrlOptions _externalUrlOptions;
    private readonly IAccountServiceApiClient _accountServiceApiClient;
    private readonly FrontEndAccountManagementOptions _frontEndAccountManagement;

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
        IOptions<FrontEndAccountManagementOptions> frontendAccountManagement,
        IOptions<ExternalUrlOptions> externalUrlOptions,
        IAccountServiceApiClient accountServiceApiClient)
    {
        _logger = logger;
        _reprocessorService = reprocessorService;
        _sessionManager = sessionManager;
        _organisationAccessor = organisationAccessor;
        _accountServiceApiClient = accountServiceApiClient;
        _linksConfig = linksConfig.Value;
        _frontEndAccountCreation = frontendAccountCreation.Value;
        _frontEndAccountManagement = frontendAccountManagement.Value;
        _externalUrlOptions = externalUrlOptions.Value;
    }

    public async Task<IActionResult> Index()
    {
        var user = _organisationAccessor.OrganisationUser;

        if (user?.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(AddOrganisation));
        }

        var existingRegistration = await _reprocessorService.Registrations.GetByOrganisationAsync((int)ApplicationType.Reprocessor, user.GetOrganisationId()!.Value);

        if (existingRegistration is not null)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
            session!.SetFromExisting(existingRegistration);
            await _sessionManager.SaveSessionAsync(HttpContext.Session, session);
        }

        //if (_organisationAccessor.Organisations.Count > 1)
        //{
        //    return RedirectToAction(nameof(SelectOrganisation));
        //}

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
        var user = _organisationAccessor.OrganisationUser;
        if (User.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var userData = user.GetUserData();
        var organisation = user.GetUserData().Organisations[0];
        
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session);
        
        string? successMessage = null;
        var removalInfo = session.ReExAccountManagement.ReExRemoveUserJourney;

        if (removalInfo != null && removalInfo.IsRemoved)
        {
            successMessage = $"{removalInfo.FirstName} {removalInfo.LastName} has been successfully removed as a {removalInfo.Role} on behalf of {organisation.Name} and will be shortly notified about their status.";

            // clear it after use (to avoid repeat on refresh)
            session.ReExAccountManagement.ReExRemoveUserJourney = null;
            await _sessionManager.SaveSessionAsync(HttpContext.Session, session);
        }
        
        var teamMembersModel = await _accountServiceApiClient.GetTeamMembersForOrganisationAsync(organisation.Id.ToString(), userData.ServiceRoleId);

        var teamMembers = teamMembersModel.Select(member => new TeamMembersResponseModel
        {
            PersonId = member.PersonId,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            ConnectionId = member.ConnectionId,

            Enrolments = member.Enrolments.Select(e => new TeamMemberEnrolments
            {
                ServiceRoleId = e.ServiceRoleId,
                ServiceRoleKey = e.ServiceRoleKey,
                EnrolmentStatusId = e.EnrolmentStatusId,
                EnrolmentStatusName = e.EnrolmentStatusName,
                AddedBy = e.AddedBy,
                EnrolmentId = e.EnrolmentId,
                ViewDetails = new Uri($"{_frontEndAccountManagement.BaseUrl}/organisation/{organisation.Id}/person/{member.PersonId}/enrolment/{e.EnrolmentId}", uriKind: UriKind.Absolute)
            }).ToList() ?? []
        }).ToList() ?? [];
        
        var teamViewModel = new TeamViewModel
        {
            OrganisationName = organisation.Name,
            OrganisationNumber = organisation.OrganisationNumber,
            OrganisationExternalId = organisation.Id,
            AddNewUser = new Uri($"{_frontEndAccountManagement.BaseUrl}{_linksConfig.AddNewUser}/organisation/{organisation.Id}", uriKind: UriKind.Absolute),
            AboutRolesAndPermissions = _linksConfig.AboutRolesAndPermissions,

            UserServiceRoles = organisation.Enrolments?.Select(x => x.ServiceRole).Where(role => !string.IsNullOrWhiteSpace(role)).Distinct().ToList(),

            TeamMembers = teamMembers
        };

        var viewModel = new HomeViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            OrganisationName = organisation.Name,
            OrganisationNumber = organisation.OrganisationNumber,
            ApplyForRegistration = _linksConfig.ApplyForRegistration,
            ViewApplications = _linksConfig.ViewApplications,
            RegistrationData = await GetRegistrationDataAsync(organisation.Id),
            AccreditationData = await GetAccreditationDataAsync(organisation.Id),
            SwitchOrManageOrganisation = _linksConfig.SwitchOrManageOrganisationLink,
            HasMultiOrganisations = userData.NumberOfOrganisations > 1,
            TeamViewModel = teamViewModel,
            SuccessMessage = successMessage
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

    private async Task<List<RegistrationDataViewModel>> GetRegistrationDataAsync(Guid? organisationId)
    {
        var registrations = await _reprocessorService.Registrations.GetRegistrationAndAccreditationAsync(organisationId);

        return registrations.Select(r =>
        {
            return new RegistrationDataViewModel
            {
                Material = (MaterialItem)r.MaterialId,
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
        var accreditations = await _reprocessorService.Registrations.GetRegistrationAndAccreditationAsync(organisationId);

        return accreditations.Select(r =>
        {
            return new AccreditationDataViewModel
            {
                Material = (MaterialItem)r.MaterialId,
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