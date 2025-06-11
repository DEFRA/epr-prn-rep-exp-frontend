using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LinksConfig _linksConfig;
    private readonly IRegistrationService _registrationService;

    public static class RouteIds
    {
        public const string ManageOrganisation = "home.manage-organisation";
    }

    public HomeController(ILogger<HomeController> logger, IOptions<LinksConfig> linksConfig, IRegistrationService registrationService)
    {
        _logger = logger;
        _linksConfig = linksConfig.Value;
        _registrationService = registrationService;
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

    [ExcludeFromCodeCoverage(Justification = "Logic for this is going to be defined on future sprint")]
    [HttpGet]
    [Route(PagePaths.AddOrganisation)]
    public IActionResult AddOrganisation()
    {
        return Ok("This is place holder for add organisation logic which need new view saying you don't have any org add new org and still on discussion");
    }

    [HttpGet]
    [Route(PagePaths.ManageOrganisation, Name = RouteIds.ManageOrganisation)]
    public async Task<IActionResult> ManageOrganisation()
    {
        var userData = User.GetUserData();
        var organisation = userData.Organisations[0];

        var viewModel = new HomeViewModel
        {
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            OrganisationName = organisation.Name,
            OrganisationNumber = organisation.OrganisationNumber,
            ApplyForRegistration = _linksConfig.ApplyForRegistration,
            ViewApplications = _linksConfig.ViewApplications,
            RegistrationData = await GetRegistrationDataAsync(organisation.Id),
            AccreditationData = null
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

    private async Task<List<RegistrationDataViewModel>> GetRegistrationDataAsync(Guid? organisationId)
    {
        var registrations = await _registrationService.GetRegistrationAndAccreditationAsync(organisationId);

        return registrations.Select(r =>
        {
            string statusText = FormatStatus(r.RegistrationStatus);

            // Get the appropriate CSS class using the new private method
            string cssClass = GetStatusCssClass((RegistrationStatus)r.RegistrationStatus);

            // Special handling for 'Granted' text, which might override the general statusText
            if ((RegistrationStatus)r.RegistrationStatus == RegistrationStatus.Granted)
            {
                statusText = $"{r.Year} Granted";
            }

            // Wrap the status text in the appropriate GOV.UK tag HTML
            string formattedStatus = string.IsNullOrEmpty(cssClass) ?
                                     statusText :
                                     $"<strong class=\"{cssClass}\">{statusText}</strong>";

            return new RegistrationDataViewModel
            {
                Material = $"{r.Material}<br />{r.ApplicationType}",
                SiteAddress = $"{r.ReprocessingSiteAddress?.AddressLine1}, {r.ReprocessingSiteAddress?.TownCity}",
                RegistrationStatus = formattedStatus,
                Action = (RegistrationStatus)r.RegistrationStatus == RegistrationStatus.InProgress ? "<a href=\"/registration/reprocessor-registration-task-list\">Continue</a>" : ""
            };
        }).ToList();
    }

    private static string FormatStatus<TEnum>(TEnum status) where TEnum : Enum
    {
        string statusString = status.ToString();
        // Insert spaces before uppercase letters (except the first character)
        string formatted = Regex.Replace(statusString, "([A-Z])", " $1").Trim();

        // Convert to title case for consistent readability
        System.Globalization.TextInfo ti = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        formatted = ti.ToTitleCase(formatted.ToLower());

        return formatted;
    }

    private static string GetStatusCssClass(RegistrationStatus status)
    {
        return status switch
        {
            RegistrationStatus.InProgress => "govuk-tag govuk-tag--blue",
            RegistrationStatus.Granted => "govuk-tag govuk-tag--green",
            RegistrationStatus.Completed => "govuk-tag govuk-tag--purple", // Example
            RegistrationStatus.Submitted => "govuk-tag govuk-tag--yellow", // Example
            // Add more cases here for other specific tag colors
            _ => "govuk-tag govuk-tag--grey" // Default tag color for unhandled statuses
        };
    }
}
