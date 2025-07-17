using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.ViewModels.Team;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[SuppressMessage("Major Code Smell", "S107:HomeController Methods should not have too many parameters", Justification = "Its Allowed for now in this case")]
public class HomeController : Controller
{
    private readonly IReprocessorService _reprocessorService;
    private readonly ISessionManager<ReprocessorRegistrationSession> _sessionManager;
    private readonly ISessionManager<JourneySession> _journeySessionManager;
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
        IOptions<LinksConfig> linksConfig,
        IReprocessorService reprocessorService,
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        ISessionManager<JourneySession> journeySessionManager,
        IOrganisationAccessor organisationAccessor,
        IOptions<FrontEndAccountCreationOptions> frontendAccountCreation,
        IOptions<FrontEndAccountManagementOptions> frontendAccountManagement,
        IOptions<ExternalUrlOptions> externalUrlOptions,
        IAccountServiceApiClient accountServiceApiClient)
    {
        _reprocessorService = reprocessorService;
        _sessionManager = sessionManager;
        _journeySessionManager = journeySessionManager;
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
        if (user!.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(AddOrganisation));
        }

        var userData = user.TryGetUserData();
        var journeySession = await _journeySessionManager.GetSessionAsync(HttpContext.Session) ?? new JourneySession();
        if (!journeySession.SelectedOrganisationId.HasValue)
        {
            if (userData?.NumberOfOrganisations == 1)
            {
                journeySession.SelectedOrganisationId = user.GetOrganisationId();
                await _journeySessionManager.SaveSessionAsync(HttpContext.Session, journeySession);
            }
            else if (userData?.NumberOfOrganisations > 1)
            {
                return RedirectToAction(nameof(SelectOrganisation));
            }
        }

        var existingRegistration = await _reprocessorService.Registrations.GetByOrganisationAsync(
            (int)ApplicationType.Reprocessor,
            user.GetOrganisationId()!.Value);

        if (existingRegistration is not null)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
            session!.SetFromExisting(existingRegistration, !string.IsNullOrEmpty(_organisationAccessor.Organisations[0].CompaniesHouseNumber));
            await _sessionManager.SaveSessionAsync(HttpContext.Session, session);
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
        var user = _organisationAccessor.OrganisationUser;
        if (user!.GetOrganisationId() == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var userData = user.GetUserData();
        var journeySession = await _journeySessionManager.GetSessionAsync(HttpContext.Session) ?? new JourneySession();
        if (journeySession.SelectedOrganisationId is null)
        {
            if (userData.NumberOfOrganisations > 1)
            {
                return RedirectToAction(nameof(SelectOrganisation));
            }
            else
            {
                journeySession.SelectedOrganisationId = userData.Organisations[0].Id;
                await _journeySessionManager.SaveSessionAsync(HttpContext.Session, journeySession);
            }
        }

        var organisation = userData.Organisations.Find(o => o.Id == journeySession.SelectedOrganisationId);
      
        string? successMessage = null;
        var removalInfo = journeySession?.ReExAccountManagementSession?.ReExRemoveUserJourney;

        if (removalInfo != null && removalInfo.IsRemoved)
        {
            successMessage = $"{removalInfo.FirstName} {removalInfo.LastName} has been successfully removed as a {removalInfo.Role} on behalf of {organisation.Name} and will be shortly notified about their status.";

            // clear it after use (to avoid repeat on refresh)
            journeySession.ReExAccountManagementSession.ReExRemoveUserJourney = null;
            await _journeySessionManager.SaveSessionAsync(HttpContext.Session, journeySession);
        }

        var teamMembersModel = await _accountServiceApiClient.GetTeamMembersForOrganisationAsync(organisation.Id.ToString(), userData.ServiceRoleId);

        var teamMembers = teamMembersModel.Select(member => new TeamMembersResponseModel
        {
            PersonId = member.PersonId,
            FirstName = member.FirstName,
            LastName = member.LastName,

            Enrolments = member.Enrolments.Select(e => new TeamMemberEnrolments
            {
                ServiceRoleId = e.ServiceRoleId,
                ServiceRoleKey = e.ServiceRoleKey,
                EnrolmentStatusId = e.EnrolmentStatusId,
                EnrolmentStatusName = e.EnrolmentStatusName,
                AddedBy = e.AddedBy,
                EnrolmentId = e.EnrolmentId,
                ViewDetails = $"{_frontEndAccountManagement.BaseUrl}/organisation/{organisation.Id}/person/{member.PersonId}/enrolment/{e.EnrolmentId}",
            }).ToList()
        }).ToList();

        var teamViewModel = new TeamViewModel
        {
            OrganisationName = organisation.Name,
            OrganisationNumber = organisation.OrganisationNumber,
            OrganisationExternalId = organisation.Id,
            AddNewUser = $"{_frontEndAccountManagement.BaseUrl}{_linksConfig.AddNewUser}/organisation/{organisation.Id}",
            AboutRolesAndPermissions = _linksConfig.AboutRolesAndPermissions,
            UserServiceRoles = organisation.Enrolments?.Select(x => x.ServiceRoleKey).Where(role => !string.IsNullOrWhiteSpace(role)).Distinct().ToList(),
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
    public async Task<IActionResult> SelectOrganisation()
    {
        var journeytSession = await _journeySessionManager.GetSessionAsync(HttpContext.Session) ?? new JourneySession();
        journeytSession.SelectedOrganisationId = null;
        await _journeySessionManager.SaveSessionAsync(HttpContext.Session, journeytSession);

        var user = _organisationAccessor.OrganisationUser!;
        var userData = user.GetUserData();

        if (userData.Organisations.Exists(o => o.Id == null))
		{
			return RedirectToAction(nameof(Index));
		}

		var viewModel = new SelectOrganisationViewModel
        {
            Organisations = [.. userData.Organisations.Select(org => new OrganisationViewModel
            {
                Id = (Guid)org.Id,
                Name = org.Name,
                OrganisationNumber = org.OrganisationNumber
            })]
        };

        return View(viewModel);
    }

    [HttpPost]
    [Route(PagePaths.SelectOrganisation)]
    public async Task<IActionResult> SelectOrganisation(SelectOrganisationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var user = _organisationAccessor.OrganisationUser!;
            var userData = user.GetUserData();
            model = new SelectOrganisationViewModel
            {
                Organisations = [.. userData.Organisations.Select(org => new OrganisationViewModel
                {
                    Id = (Guid)org.Id,
                    Name = org.Name,
                    OrganisationNumber = org.OrganisationNumber
                })]
            };
            return View(model);
        }

        var journeytSession = await _journeySessionManager.GetSessionAsync(HttpContext.Session) ?? new JourneySession();
        journeytSession.SelectedOrganisationId = model.SelectedOrganisationId;
        await _journeySessionManager.SaveSessionAsync(HttpContext.Session, journeytSession);
        return RedirectToAction(nameof(ManageOrganisation));
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
            string continueLink = string.Empty;

            if (r.ApplicationTypeId == ApplicationType.Reprocessor)
            {
                continueLink = $"{_linksConfig.RegistrationReprocessorContinueLink}/{r.Id}/{r.MaterialId}";
            }
            else if (r.ApplicationTypeId == ApplicationType.Exporter)
            {
                continueLink = $"{_linksConfig.RegistrationExporterContinueLink}/{r.Id}/{r.MaterialId}";
            }

            return new RegistrationDataViewModel
            {
                Material = (Material)r.MaterialId,
                ApplicationType = r.ApplicationTypeId,
                SiteAddress = $"{r.ReprocessingSiteAddress?.AddressLine1}, {r.ReprocessingSiteAddress?.TownCity}",
                RegistrationStatus = (RegistrationStatus)r.RegistrationStatus,
                Year = r.Year,
                RegistrationContinueLink = continueLink
            };
        }).ToList();
    }

    private async Task<List<AccreditationDataViewModel>> GetAccreditationDataAsync(Guid? organisationId)
    {
        var accreditations = await _reprocessorService.Registrations.GetRegistrationAndAccreditationAsync(organisationId);

        return accreditations.Select(r =>
        {
            string startLink;
            string continueLink;

            if (r.ApplicationTypeId == ApplicationType.Reprocessor)
            {
                startLink = $"{_linksConfig.AccreditationStartLink}/{r.ReprocessingSiteId}/{r.MaterialId}";
                continueLink = $"{_linksConfig.AccreditationReprocessorContinueLink}/{r.ReprocessingSiteId}/{r.MaterialId}";
            }
            else if (r.ApplicationTypeId == ApplicationType.Exporter)
            {
                startLink = $"{_linksConfig.AccreditationStartLink}/{r.MaterialId}";
                continueLink = $"{_linksConfig.AccreditationExporterContinueLink}/{r.MaterialId}";
            }
            else
            {
                startLink = string.Empty;
                continueLink = string.Empty;
            }

            return new AccreditationDataViewModel
            {
                Material = (Material)r.MaterialId,
                ApplicationType = r.ApplicationTypeId,
                SiteAddress = $"{r.ReprocessingSiteAddress?.AddressLine1},{r.ReprocessingSiteAddress?.TownCity}",
                AccreditationStatus = (Enums.AccreditationStatus)r.AccreditationStatus,
                Year = r.Year,
                AccreditationStartLink = startLink,
                AccreditationContinueLink = continueLink
            };
        }).ToList();
    }
}