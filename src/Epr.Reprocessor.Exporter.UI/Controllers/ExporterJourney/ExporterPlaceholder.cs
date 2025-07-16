using System;
using AutoMapper;
using Azure.Core;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Epr.Reprocessor.Exporter.UI.App.Constants.Endpoints;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.ExporterPlaceholder)]
    public class ExporterPlaceholderController(
            ILogger<ExporterPlaceholderController> logger,
            ISaveAndContinueService saveAndContinueService,
            ISessionManager<ExporterRegistrationSession> sessionManager,
            IConfiguration configuration,
            IRegistrationService registrationService) : BaseExporterController(logger, saveAndContinueService, sessionManager, configuration)
    {
        private const string LastGuidsCookieKey = "LastRegistrationGuids";

        private const string NextPageInJourney = PagePaths.ExporterWasteCarrierBrokerDealerRegistration;
        private const string CurrentPageInJourney = PagePaths.ExporterPlaceholder;
        private const string SaveAndContinueExporterPlaceholderKey = "SaveAndContinueExporterPlaceholderKey";

        private readonly IRegistrationService _registrationService = registrationService;
        private readonly IWasteCarrierBrokerDealerRefService _wasteCarrierBrokerDealerRefService = wasteCarrierBrokerDealerRefService;

        public IActionResult Index()
        {
            ViewBag.Command = "Holding";
            return View("~/Views/ExporterJourney/ExporterPlaceholder.cshtml");
        }

        [HttpGet("/start-exporter-registration")]
        public IActionResult NewExporterRegistration()
        {
            ViewBag.Command = "Start";
            ViewBag.LastGuids = GetLastRegistrationGuids(HttpContext);

            return View("~/Views/ExporterJourney/ExporterPlaceholder.cshtml");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Smell",
            "S3776:Cognitive Complexity of methods should not be too high",
            Justification = "Business logic requires multiple branches and error handling; refactoring would reduce clarity.")]
        [HttpPost]
        public async Task<IActionResult> Post(string action, string? RegistrationGuid)
        {
            Guid? registrationId = null;
            WasteCarrierBrokerDealerRefDto? dto = null;

            await InitialiseSession();

            if (action == "CreateNew")
            {
                var userData = User.GetUserData();
                var organisation = userData.Organisations.FirstOrDefault();
                var organisationId = organisation != null ? organisation.Id.Value : Guid.Empty;

                var createRegistration = new CreateRegistrationDto
                {
                    ApplicationTypeId = 1,
                    OrganisationId = organisationId,
                };

                if (organisation != null)
                {
                    createRegistration.ReprocessingSiteAddress = new AddressDto
                    {
                        AddressLine1 = organisation.BuildingNumber,
                        AddressLine2 = organisation.Street,
                        TownCity = organisation.Town,
                        County = organisation.County,
                        Country = organisation.Country,
                        PostCode = organisation.Postcode,
                        NationId = organisation.NationId,
                        GridReference = string.Empty
                    };
                }

                var response = await _registrationService.CreateAsync(createRegistration);
                registrationId = response.Id;

                if (registrationId == Guid.Empty)
                {
                    ModelState.AddModelError("RegistrationGuid", "Failed to create a new registration.");
                    ViewBag.Command = "Start";
                    ViewBag.LastGuids = GetLastRegistrationGuids(HttpContext);
                    return View("~/Views/ExporterJourney/ExporterPlaceholder.cshtml");
                }

                RememberRegistrationGuid(HttpContext, registrationId.Value);
                
            }
            else if (action == "RecallExisting")
            {
                // Validate GUID
                if (string.IsNullOrWhiteSpace(RegistrationGuid) || !Guid.TryParse(RegistrationGuid, out var parsedGuid))
                {
                    ModelState.AddModelError("RegistrationGuid", "Please enter a valid registration GUID.");
                    ViewBag.Command = "Start";
                    ViewBag.LastGuids = GetLastRegistrationGuids(HttpContext);
                    return View("~/Views/ExporterJourney/ExporterPlaceholder.cshtml");
                }

                registrationId = parsedGuid;

                // Try to get the registration DTO
                try
                {
                    dto = await _wasteCarrierBrokerDealerRefService.GetByRegistrationId(registrationId.Value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to retrieve registration {RegistrationId}", registrationId);
                }

                if (dto == null)
                {
                    ModelState.AddModelError("RegistrationGuid", "No registration found for the provided GUID.");
                    ViewBag.Command = "Start";
                    ViewBag.LastGuids = GetLastRegistrationGuids(HttpContext);
                    return View("~/Views/ExporterJourney/ExporterPlaceholder.cshtml");
                }

                RememberRegistrationGuid(HttpContext, registrationId.Value);
            }
            else
            {
                // Unknown action
                return BadRequest();
            }

            await GetRegistrationIdAsync(registrationId.Value);

            await SetExplicitBackLink(PagePaths.ManageOrganisation, PagePaths.ExporterPlaceholder);

            await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, nameof(ExporterPlaceholderController),
                nameof(Index), null, SaveAndContinueExporterPlaceholderKey);

            return Redirect(NextPageInJourney);
        }

        private static List<string> GetLastRegistrationGuids(HttpContext context)
        {
            var cookie = context.Request.Cookies[LastGuidsCookieKey];
            return string.IsNullOrEmpty(cookie)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(cookie);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Security",
            "S2092:Cookies should be created with the 'secure' flag",
            Justification = "This cookie does not contain sensitive data and is being used for test purposes only")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Security",
            "S3330:Cookies should be created with the 'HttpOnly' flag",
            Justification = "This cookie does not contain sensitive data and is being used for test purposes only")]
        private static void RememberRegistrationGuid(HttpContext context, Guid guid)
        {
            var cookie = context.Request.Cookies[LastGuidsCookieKey];
            var list = string.IsNullOrEmpty(cookie)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(cookie);

            list.Remove(guid.ToString());
            list.Insert(0, guid.ToString());
            if (list.Count > 5)
                list = list.Take(5).ToList();

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true, 
                HttpOnly = false    
            };

            context.Response.Cookies.Append(
                LastGuidsCookieKey,
                System.Text.Json.JsonSerializer.Serialize(list),
                options
            );
        }
    }
}
