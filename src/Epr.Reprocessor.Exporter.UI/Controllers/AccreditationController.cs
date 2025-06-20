﻿using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Controllers.ControllerExtensions;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.AccreditationLanding)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public class AccreditationController(
        IStringLocalizer<SharedResources> sharedLocalizer,
        IOptions<ExternalUrlOptions> externalUrlOptions,
        IValidationService validationService,
        IAccountServiceApiClient accountServiceApiClient,
        IAccreditationService accreditationService) : Controller
    {
        public static class RouteIds
        {
            public const string EnsureAccreditation = "accreditation.ensure-accreditation";
            public const string SelectPrnTonnage = "accreditation.prns-plan-to-issue";
            public const string SelectPernTonnage = "accreditation.perns-plan-to-issue";
            public const string SelectAuthorityPRNs = "accreditation.select-authority-for-people-prns";
            public const string SelectAuthorityPERNs = "accreditation.select-authority-for-people-perns";
            public const string MoreDetailOnBusinessPlanPRNs = "accreditation.more-detail-on-business-plan-prns";
            public const string MoreDetailOnBusinessPlanPERNs = "accreditation.more-detail-on-business-plan-perns";
            public const string CheckAnswersPRNs = "accreditation.check-answers-prns";
            public const string CheckAnswersPERNs = "accreditation.check-answers-perns";
            public const string ApplicationSaved = "accreditation.application-saved";
            public const string CheckBusinessPlanPRN = "accreditation.check-business-plan-prn";
            public const string CheckBusinessPlanPERN = "accreditation.check-business-plan-pern";
            public const string AccreditationTaskList = "accreditation.reprocessor-accreditation-task-list";
            public const string ExporterAccreditationTaskList = "accreditation.exporter-accreditation-task-list";
            public const string ReprocessorSamplingAndInspectionPlan = "accreditation.reprocessor-sampling-inspection-plan";
            public const string ExporterSamplingAndInspectionPlan = "accreditation.exporter-sampling-inspection-plan";
            public const string BusinessPlanPercentages = "accreditation.busines-plan-percentages";
            public const string ApplyingFor2026Accreditation = "accreditation.applying-for-2026-accreditation";
            public const string Declaration = "accreditation.declaration";
            public const string ReprocessorConfirmApplicationSubmission = "accreditation.reprocessor-application-submitted";
            public const string ExporterConfirmaApplicationSubmission = "accreditation.exporter-application-submitted";            
            public const string SelectOverseasSites = "accreditation.select-overseas-sites";
            public const string NotAnApprovedPerson = "accreditation.complete-not-submit-accreditation-application";
        }

        [HttpGet(PagePaths.ApplicationSaved, Name = RouteIds.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["ApplicationTitle"] = sharedLocalizer["application_title_accreditation"];
            base.OnActionExecuting(context);
        }

        [HttpGet("clear-down-database")]
        public async Task<IActionResult> ClearDownDatabase()
        {
            // Temporary: Aid to QA whilst Accreditation uses in-memory database.
            await accreditationService.ClearDownDatabase();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet(PagePaths.EnsureAccreditation, Name = RouteIds.EnsureAccreditation)]
        public async Task<IActionResult> EnsureAccreditation(
            [FromRoute] int materialId,
            [FromRoute] int applicationTypeId)
        {
            var userData = User.GetUserData();
            var organisationId = userData.Organisations[0].Id.Value;

            var accreditationId = await accreditationService.GetOrCreateAccreditation(organisationId, materialId, applicationTypeId);

            return applicationTypeId switch
            {
                1 => RedirectToRoute(RouteIds.AccreditationTaskList, new { accreditationId }),
                2 => RedirectToRoute(RouteIds.ExporterAccreditationTaskList, new { accreditationId }),
                _ => BadRequest("Invalid application type supplied.")
            };
        }

        [HttpGet, Route(PagePaths.NotAnApprovedPerson, Name = RouteIds.NotAnApprovedPerson)]
        public async Task<IActionResult> NotAnApprovedPerson()
        {
            var userData = User.GetUserData();
            var organisationId = userData.Organisations[0].Id.ToString();

            var usersApproved = await accountServiceApiClient.GetUsersForOrganisationAsync(organisationId, (int)ServiceRole.Approved);
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var approvedPersons = new List<string>();
            foreach (var user in usersApproved)
            {
                approvedPersons.Add($"{user.FirstName} {user.LastName}");
            }

            var viewModel = new NotAnApprovedPersonViewModel()
            {
                ApprovedPersons = approvedPersons
            };
            return View(viewModel);
        }

        [HttpGet(PagePaths.CalendarYear)]
        public IActionResult CalendarYear() => View(new CalendarYearViewModel { NpwdLink = externalUrlOptions.Value.NationalPackagingWasteDatabase });

        [HttpGet(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage), HttpGet(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage)]
        public async Task<IActionResult> PrnTonnage([FromRoute] Guid accreditationId)
        {
            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList,
                new { AccreditationId = accreditationId });

            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            var model = new PrnTonnageViewModel()
            {
                AccreditationId = accreditation.ExternalId,
                MaterialName = accreditation.MaterialName.ToLower(),
                PrnTonnage = accreditation.PrnTonnage,
                Subject = HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ? "PRN" : "PERN",
                FormPostRouteName = HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ?
                    AccreditationController.RouteIds.SelectPrnTonnage :
                    AccreditationController.RouteIds.SelectPernTonnage
            };

            return View(model);
        }

        [HttpPost(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage), HttpPost(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage)]
        public async Task<IActionResult> PrnTonnage(PrnTonnageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BackLinkToDisplay = Url.RouteUrl(
                    HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList,
                    new { AccreditationId = model.AccreditationId });

                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
            accreditation.PrnTonnage = model.PrnTonnage;

            var request = GetAccreditationRequestDto(accreditation);
            await accreditationService.UpsertAccreditation(request);

            return model.Action switch
            {
                "continue" => HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ?
                    RedirectToRoute(RouteIds.SelectAuthorityPRNs, new { model.AccreditationId }) :
                    RedirectToRoute(RouteIds.SelectAuthorityPERNs, new { model.AccreditationId }),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }


        [HttpGet(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
            HttpGet(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]
        public async Task<IActionResult> SelectAuthority([FromRoute] Guid accreditationId)
        {
            var model = new SelectAuthorityViewModel();

            model.Accreditation = await accreditationService.GetAccreditation(accreditationId);
            model.PrnIssueAuthorities = await accreditationService.GetAccreditationPrnIssueAuths(accreditationId);
            model.HomePageUrl = Url.Action(action: "Index", controller: nameof(HomeController).RemoveControllerFromName());

            ValidateRouteForApplicationType(model.ApplicationType);

            // set viewbag back link based on application type
            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                routeName: (model.ApplicationType == ApplicationType.Reprocessor ? RouteIds.SelectPrnTonnage : RouteIds.SelectPernTonnage),
                values: new { accreditationId = accreditationId });

            model.SiteAddress = "23 Ruby Street";

            model.SelectedAuthorities = model.PrnIssueAuthorities?.Select(x => x.PersonExternalId.ToString()).ToList() ?? new List<string>();
            
            var userData = User.GetUserData();

            List<ManageUserDto> users = new();

            users.AddRange(await accreditationService.GetOrganisationUsers(userData, true));

            model.Authorities.AddRange(users.Select(x => new SelectListItem
            {
                Value = x.PersonId.ToString(),
                Text = x.FirstName + " " + x.LastName,
                Group = new SelectListGroup { Name = x.Email }
            }
                ).ToList());

            return View(model);
        }


        [HttpPost(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
            HttpPost(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]

        public async Task<IActionResult> SelectAuthority(SelectAuthorityViewModel model)
        {
            // set viewbag back link based on application type
            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                routeName: (model.ApplicationType == ApplicationType.Reprocessor ? RouteIds.SelectPrnTonnage : RouteIds.SelectPernTonnage),
                values: new { accreditationId = model.Accreditation.ExternalId });

            var validationResult = await validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            List<AccreditationPrnIssueAuthRequestDto> requestDtos = new List<AccreditationPrnIssueAuthRequestDto>();
            foreach (var authority in model.SelectedAuthorities)
            {
                requestDtos.Add(new AccreditationPrnIssueAuthRequestDto
                {
                    PersonExternalId = Guid.Parse(authority),

                });
            }

            await accreditationService.ReplaceAccreditationPrnIssueAuths(model.Accreditation.ExternalId, requestDtos);

            return model.Action switch
            {
                "continue" => model.ApplicationType == ApplicationType.Reprocessor ? RedirectToRoute(RouteIds.CheckAnswersPRNs, new { accreditationId = model.Accreditation.ExternalId }) : RedirectToRoute(RouteIds.CheckAnswersPERNs, new { accreditationId = model.Accreditation.ExternalId }),
          
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs),
            HttpGet(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs)]
        public async Task<IActionResult> CheckAnswers([FromRoute] Guid accreditationId)
        {
            // Get accreditation object
            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            // Get selected users to issue prns
            var prnIssueAuths = await accreditationService.GetAccreditationPrnIssueAuths(accreditationId);

            // Get organisation users
            var users = await accreditationService.GetOrganisationUsers(User.GetUserData(), true);

            var authPersonIds = prnIssueAuths?.Select(a => a.PersonExternalId).ToHashSet();

            var authorisedSelectedUsers = users != null && authPersonIds != null
                ? users
                    .Where(u => authPersonIds.Contains(u.PersonId))
                    .Select(u => u.FirstName + " " + u.LastName)
                : null;

            var subject = GetSubject(RouteIds.CheckAnswersPRNs);

            var isPrnRoute = subject == "PRN";

            var model = new CheckAnswersViewModel
            {
                AccreditationId = accreditationId,
                PrnTonnage = accreditation?.PrnTonnage,
                AuthorisedUsers = authorisedSelectedUsers != null ? string.Join(", ", authorisedSelectedUsers) : string.Empty,
                Subject = subject,
                TonnageChangeRoutePath = isPrnRoute ? RouteIds.SelectPrnTonnage : RouteIds.SelectPernTonnage,
                AuthorisedUserChangeRoutePath = isPrnRoute ? RouteIds.SelectAuthorityPRNs : RouteIds.SelectAuthorityPERNs,
                FormPostRouteName = isPrnRoute ? RouteIds.CheckAnswersPRNs : RouteIds.CheckAnswersPERNs,
            };

            SetBackLink(isPrnRoute ? RouteIds.SelectAuthorityPRNs : RouteIds.SelectAuthorityPERNs, model.AccreditationId);

            return View(model);
        }

        [HttpPost(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs),
            HttpPost(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs)]
        public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            switch (model.Action)
            {
                case "continue":
                    var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
                    var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
                    accreditationRequestDto.PrnTonnageAndAuthoritiesConfirmed = true;
                    await accreditationService.UpsertAccreditation(accreditationRequestDto);
                    if (model.Subject == "PERN")
                    {
                        return RedirectToRoute(RouteIds.ExporterAccreditationTaskList, new { model.AccreditationId });
                    }
                    else
                    {
                        return RedirectToRoute(RouteIds.AccreditationTaskList, new { model.AccreditationId });
                    }

                case "save":
                    return RedirectToRoute(RouteIds.ApplicationSaved);

                default:
                    return BadRequest("Invalid action supplied.");
            }
        }

        [HttpGet(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinessPlanPercentages)]
        public async Task<IActionResult> BusinessPlan(Guid accreditationId)
        {
            var accreditation = await accreditationService.GetAccreditation(accreditationId);
            var isPrn = accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor;

            ViewBag.BackLinkToDisplay = Url.RouteUrl(isPrn ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList, new { AccreditationId = accreditationId });

            var model = new BusinessPlanViewModel()
            {
                ExternalId = accreditation.ExternalId,
                MaterialName = accreditation.MaterialName,
                Subject = isPrn ? "PRN" : "PERN",
                InfrastructurePercentage = GetBusinessPlanPercentage(accreditation.InfrastructurePercentage),
                PackagingWastePercentage = GetBusinessPlanPercentage(accreditation.PackagingWastePercentage),
                BusinessCollectionsPercentage = GetBusinessPlanPercentage(accreditation.BusinessCollectionsPercentage),
                CommunicationsPercentage = GetBusinessPlanPercentage(accreditation.CommunicationsPercentage),
                OtherPercentage = GetBusinessPlanPercentage(accreditation.OtherPercentage),
                NewMarketsPercentage = GetBusinessPlanPercentage(accreditation.NewMarketsPercentage),
                NewUsesPercentage = GetBusinessPlanPercentage(accreditation.NewUsesPercentage),
            };

            return View(model);
        }

        [HttpPost(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinessPlanPercentages)]
        public async Task<IActionResult> BusinessPlan(BusinessPlanViewModel model)
        {
            var isPrn = model.Subject == "PRN";

            ViewBag.BackLinkToDisplay = Url.RouteUrl(isPrn ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList, new { AccreditationId = model.ExternalId });

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.ExternalId);
            var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
            accreditationRequestDto.BusinessCollectionsPercentage = GetBusinessPlanPercentage(model.BusinessCollectionsPercentage);
            accreditationRequestDto.CommunicationsPercentage = GetBusinessPlanPercentage(model.CommunicationsPercentage);
            accreditationRequestDto.InfrastructurePercentage = GetBusinessPlanPercentage(model.InfrastructurePercentage);
            accreditationRequestDto.NewMarketsPercentage = GetBusinessPlanPercentage(model.NewMarketsPercentage);
            accreditationRequestDto.NewUsesPercentage = GetBusinessPlanPercentage(model.NewUsesPercentage);
            accreditationRequestDto.PackagingWastePercentage = GetBusinessPlanPercentage(model.PackagingWastePercentage);
            accreditationRequestDto.OtherPercentage = GetBusinessPlanPercentage(model.OtherPercentage);
            accreditationRequestDto.BusinessPlanConfirmed = false;

            await accreditationService.UpsertAccreditation(accreditationRequestDto);

            // Navigate to next page
            switch (model.Action)
            {
                case "continue":
                    {
                        return RedirectToRoute(isPrn ? RouteIds.MoreDetailOnBusinessPlanPRNs : RouteIds.MoreDetailOnBusinessPlanPERNs, new { accreditationId = model.ExternalId });
                    }
                case "save":
                    {
                        return RedirectToRoute(RouteIds.ApplicationSaved);
                    }
                default:
                    return BadRequest("Invalid action supplied.");
            }
        }

        [HttpGet(PagePaths.MoreDetailOnBusinessPlanPRNs, Name = RouteIds.MoreDetailOnBusinessPlanPRNs),
            HttpGet(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan(Guid accreditationId)
        {
            var accreditation = await accreditationService.GetAccreditation(accreditationId);
            ValidateRouteForApplicationType((ApplicationType)accreditation.ApplicationTypeId);

            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.BusinessPlanPercentages, new { AccreditationId = accreditationId });

            var model = new MoreDetailOnBusinessPlanViewModel()
            {
                AccreditationId = accreditation.ExternalId,
                ShowInfrastructure = accreditation.InfrastructurePercentage > 0,
                Infrastructure = accreditation.InfrastructureNotes,
                ShowPriceSupport = accreditation.PackagingWastePercentage > 0,
                PriceSupport = accreditation.PackagingWasteNotes,
                ShowBusinessCollections = accreditation.BusinessCollectionsPercentage > 0,
                BusinessCollections = accreditation.BusinessCollectionsNotes,
                ShowCommunications = accreditation.CommunicationsPercentage > 0,
                Communications = accreditation.CommunicationsNotes,
                ShowNewMarkets = accreditation.NewMarketsPercentage > 0,
                NewMarkets = accreditation.NewMarketsNotes,
                ShowNewUses = accreditation.NewUsesPercentage > 0,
                NewUses = accreditation.NewUsesNotes,
                ShowOther = accreditation.OtherPercentage > 0,
                Other = accreditation.OtherNotes,
                ApplicationTypeId = accreditation.ApplicationTypeId,
                Subject = accreditation.ApplicationTypeId ==  (int)ApplicationType.Reprocessor? "PRN" : "PERN",
                FormPostRouteName = accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ?
                    AccreditationController.RouteIds.MoreDetailOnBusinessPlanPRNs :
                    AccreditationController.RouteIds.MoreDetailOnBusinessPlanPERNs
            };

            return View(model);
        }

        [HttpPost(PagePaths.MoreDetailOnBusinessPlanPRNs, Name = RouteIds.MoreDetailOnBusinessPlanPRNs),
            HttpPost(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan(MoreDetailOnBusinessPlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.BusinessPlanPercentages, new { AccreditationId = model.AccreditationId });

                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
            var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
            accreditationRequestDto.InfrastructureNotes = accreditation.InfrastructurePercentage > 0 ? model.Infrastructure : null;
            accreditationRequestDto.PackagingWasteNotes = accreditation.PackagingWastePercentage > 0 ? model.PriceSupport : null;
            accreditationRequestDto.BusinessCollectionsNotes = accreditation.BusinessCollectionsPercentage > 0 ? model.BusinessCollections : null;
            accreditationRequestDto.CommunicationsNotes = accreditation.CommunicationsPercentage > 0 ? model.Communications : null;
            accreditationRequestDto.NewMarketsNotes = accreditation.NewMarketsPercentage > 0 ? model.NewMarkets : null;
            accreditationRequestDto.NewUsesNotes = accreditation.NewUsesPercentage > 0 ? model.NewUses : null;
            accreditationRequestDto.OtherNotes = accreditation.OtherPercentage > 0 ? model.Other : null;
            accreditationRequestDto.BusinessPlanConfirmed = false;

            await accreditationService.UpsertAccreditation(accreditationRequestDto);

            return model.Action switch
            {
                "continue" => RedirectToRoute(
                    accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ? RouteIds.CheckBusinessPlanPRN : RouteIds.CheckBusinessPlanPERN,
                    new { accreditationId = model.AccreditationId }),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(template: PagePaths.ApplyForAccreditation, Name = PagePaths.ApplyForAccreditation)]
        public IActionResult ApplyforAccreditation() => View(new ApplyForAccreditationViewModel());


        [HttpGet(PagePaths.AccreditationTaskList, Name = RouteIds.AccreditationTaskList), HttpGet(PagePaths.ExporterAccreditationTaskList, Name = RouteIds.ExporterAccreditationTaskList)]
        public async Task<IActionResult> TaskList([FromRoute] Guid accreditationId, bool isFileUploadSimulated = false)
        {
            var subject = GetSubject(RouteIds.AccreditationTaskList);
            ViewBag.Subject = subject;


            var userData = User.GetUserData();
            var organisationId = userData.Organisations[0].Id.ToString();
            var approvedPersons = new List<string>();

            var isAuthorisedUser = userData.ServiceRoleId == (int)ServiceRole.Approved || userData.ServiceRoleId == (int)ServiceRole.Delegated;
           
            ViewBag.BackLinkToDisplay = Url.RouteUrl(isAuthorisedUser ? HomeController.RouteIds.ManageOrganisation : RouteIds.NotAnApprovedPerson);

            if (!isAuthorisedUser)
            {
                var usersApproved = await accountServiceApiClient.GetUsersForOrganisationAsync(organisationId, (int)ServiceRole.Approved);
                if (usersApproved != null)
                {
                    approvedPersons.AddRange(usersApproved.Select(user => $"{user.FirstName} {user.LastName}"));
                }
            }

            // Get accreditation object
            var accreditation = await accreditationService.GetAccreditation(accreditationId);
            
            // Get selected users to issue prns
            var prnIssueAuths = await accreditationService.GetAccreditationPrnIssueAuths(accreditationId);

            var isPrnRoute = subject == "PRN";

            var model = new TaskListViewModel
            {
                Accreditation= accreditation,
                
                IsApprovedUser = isAuthorisedUser,
                TonnageAndAuthorityToIssuePrnStatus = GetTonnageAndAuthorityToIssuePrnStatus(accreditation?.PrnTonnage, accreditation?.PrnTonnageAndAuthoritiesConfirmed ?? false, prnIssueAuths),
                BusinessPlanStatus = GetBusinessPlanStatus(accreditation),
                AccreditationSamplingAndInspectionPlanStatus = GetAccreditationSamplingAndInspectionPlanStatus(isFileUploadSimulated),
                PeopleCanSubmitApplication = new PeopleAbleToSubmitApplicationViewModel { ApprovedPersons = approvedPersons },
                PrnTonnageRouteName = isPrnRoute ? RouteIds.SelectPrnTonnage : RouteIds.SelectPernTonnage,
                SamplingInspectionRouteName = isPrnRoute ? RouteIds.ReprocessorSamplingAndInspectionPlan : RouteIds.ExporterSamplingAndInspectionPlan,
                SelectOverseasSitesRouteName = RouteIds.SelectOverseasSites,
            };
            ValidateRouteForApplicationType(model.ApplicationType);


            return View(model);
        }

        [HttpGet(PagePaths.CheckBusinessPlanPRN, Name = RouteIds.CheckBusinessPlanPRN), HttpGet(PagePaths.CheckBusinessPlanPERN, Name = RouteIds.CheckBusinessPlanPERN)]
        public async Task<IActionResult> ReviewBusinessPlan(Guid accreditationId)
        {
            var accreditation = await accreditationService.GetAccreditation(accreditationId);
            ValidateRouteForApplicationType((ApplicationType)accreditation.ApplicationTypeId);

            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ? RouteIds.MoreDetailOnBusinessPlanPRNs : RouteIds.MoreDetailOnBusinessPlanPERNs,
                new { AccreditationId = accreditationId });

            var model = new ReviewBusinessPlanViewModel()
            {
                AccreditationId = accreditation.ExternalId,
                ApplicationTypeId = accreditation.ApplicationTypeId,
                Subject = accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ? "PRN" : "PERN",
                InfrastructurePercentage = accreditation.InfrastructurePercentage ?? 0,
                PriceSupportPercentage = accreditation.PackagingWastePercentage ?? 0,
                BusinessCollectionsPercentage = accreditation.BusinessCollectionsPercentage ?? 0,
                CommunicationsPercentage = accreditation.CommunicationsPercentage ?? 0,
                NewMarketsPercentage = accreditation.NewMarketsPercentage ?? 0,
                NewUsesPercentage = accreditation.NewUsesPercentage ?? 0,
                OtherPercentage = accreditation.OtherPercentage ?? 0,
                InfrastructureNotes = accreditation.InfrastructureNotes,
                PriceSupportNotes = accreditation.PackagingWasteNotes,
                BusinessCollectionsNotes = accreditation.BusinessCollectionsNotes,
                CommunicationsNotes = accreditation.CommunicationsNotes,
                NewMarketsNotes = accreditation.NewMarketsNotes,
                NewUsesNotes = accreditation.NewUsesNotes,
                OtherNotes = accreditation.OtherNotes,
                BusinessPlanUrl = Url.RouteUrl(RouteIds.BusinessPlanPercentages, new { AccreditationId = accreditationId }),
                MoreDetailOnBusinessPlanUrl = Url.RouteUrl(accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ?
                    RouteIds.MoreDetailOnBusinessPlanPRNs : RouteIds.MoreDetailOnBusinessPlanPERNs, new { AccreditationId = accreditationId }),
            };

            return View(model);
        }

        [HttpPost(PagePaths.CheckBusinessPlanPRN, Name = RouteIds.CheckBusinessPlanPRN), HttpPost(PagePaths.CheckBusinessPlanPERN, Name = RouteIds.CheckBusinessPlanPERN)]
        public async Task<IActionResult> ReviewBusinessPlan(ReviewBusinessPlanViewModel model)
        {
            switch (model.Action)
            {
                case "continue":
                    var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
                    var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
                    accreditationRequestDto.BusinessPlanConfirmed = true;
                    await accreditationService.UpsertAccreditation(accreditationRequestDto);

                    return RedirectToRoute(model.ApplicationTypeId == (int)ApplicationType.Reprocessor ?
                        RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList,
                        new { accreditationId = model.AccreditationId });

                case "save":
                    return RedirectToRoute(RouteIds.ApplicationSaved);

                default:
                    return BadRequest("Invalid action supplied.");
            }
        }

        [HttpGet(PagePaths.AccreditationSamplingAndInspectionPlan)]
        public async Task<IActionResult> SamplingAndInspectionPlan()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var viewModel = new SamplingAndInspectionPlanViewModel()
            {
                MaterialName = "steel",
                UploadedFiles = new List<FileUploadViewModel>
            {
                new FileUploadViewModel
                {
                    FileName = "SamplingAndInspectionXYZReprocessingSteel.pdf",
                    DateUploaded = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    UploadedBy = "Jane Winston"
                }
            }
            };

            return View(viewModel);
        }

        [HttpGet(PagePaths.ApplyingFor2026Accreditation, Name = RouteIds.ApplyingFor2026Accreditation)]
        public IActionResult ApplyingFor2026Accreditation(Guid accreditationId)
        {

            var userData = User.GetUserData();
            var isAuthorisedUser = userData.ServiceRoleId == (int)ServiceRole.Approved || userData.ServiceRoleId == (int)ServiceRole.Delegated;       
            ViewBag.BackLinkToDisplay = Url.RouteUrl(isAuthorisedUser ? HomeController.RouteIds.ManageOrganisation : RouteIds.NotAnApprovedPerson);          

            return View(accreditationId);
        }

        [HttpGet(PagePaths.AccreditationDeclaration, Name = RouteIds.Declaration)]
        public async Task<IActionResult> Declaration([FromRoute] Guid accreditationId)
        {
            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                accreditation.ApplicationTypeId == (int)ApplicationType.Reprocessor ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList,
                new { AccreditationId = accreditationId });

            var model = new DeclarationViewModel()
            {
                AccreditationId = accreditation.ExternalId,
                CompanyName = User.GetUserData().Organisations[0].Name, // MLS - Assume user has one Organisation.
                ApplicationTypeId = accreditation.ApplicationTypeId,
            };

            return View(model);
        }

        [HttpPost(PagePaths.AccreditationDeclaration, Name = RouteIds.Declaration)]
        public async Task<IActionResult> Declaration(DeclarationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BackLinkToDisplay = Url.RouteUrl(
                    model.ApplicationTypeId == (int)ApplicationType.Reprocessor ? RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList,
                    new { AccreditationId = model.AccreditationId});

                return View(model);
            }
            bool reprocessor = model.ApplicationTypeId == (int)ApplicationType.Reprocessor;
            var appType = reprocessor ? ApplicationType.Reprocessor : ApplicationType.Exporter;
            var organisation = User.GetUserData().Organisations[0];

            var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
            accreditation.AccreditationStatusId = (int)AccreditationStatus.Submitted;
            accreditation.DecFullName = model.FullName;
            accreditation.DecJobTitle = model.JobTitle;
            accreditation.AccreferenceNumber = accreditationService.CreateApplicationReferenceNumber(
                                               "A", organisation.NationId.Value, appType, organisation.OrganisationNumber, accreditation.MaterialName);

            var request = GetAccreditationRequestDto(accreditation);
            await accreditationService.UpsertAccreditation(request);
            var route = reprocessor ? RouteIds.ReprocessorConfirmApplicationSubmission : RouteIds.ExporterConfirmaApplicationSubmission;

            return RedirectToRoute(route, new { model.AccreditationId });
        }

        [HttpGet(PagePaths.ReprocessorAccreditationSamplingFileUpload, Name = RouteIds.ReprocessorSamplingAndInspectionPlan),
         HttpGet(PagePaths.ExporterAccreditationSamplingFileUpload, Name = RouteIds.ExporterSamplingAndInspectionPlan)]
        public async Task<IActionResult> FakeAccreditationSamplingFileUpload(Guid accreditationId)
        {
            ViewBag.AccreditationId = accreditationId;
            ViewBag.FormPostRouteName = HttpContext.GetRouteName() == RouteIds.ReprocessorSamplingAndInspectionPlan ?
                                        RouteIds.AccreditationTaskList : RouteIds.ExporterAccreditationTaskList;

            return View();
        }

        [HttpGet(PagePaths.ReprocessorApplicationSubmissionConfirmation, Name = RouteIds.ReprocessorConfirmApplicationSubmission),
         HttpGet(PagePaths.ExporterApplicationSubmissionConfirmation, Name = RouteIds.ExporterConfirmaApplicationSubmission)]
        public async Task<IActionResult> ApplicationSubmissionConfirmation([FromRoute] Guid accreditationId)
        {
            bool reprocessor = HttpContext.GetRouteName() == RouteIds.ReprocessorConfirmApplicationSubmission;
            var accreditation = await accreditationService.GetAccreditation(accreditationId);
            var applicationReferenceNumber = accreditation.AccreferenceNumber;

            if (string.IsNullOrEmpty(applicationReferenceNumber))
            {
                var appType = reprocessor ? ApplicationType.Reprocessor : ApplicationType.Exporter;
                var organisation = User.GetUserData().Organisations[0];
                applicationReferenceNumber = accreditationService.CreateApplicationReferenceNumber(
                                             "A", organisation.NationId.Value, appType, organisation.OrganisationNumber, accreditation.MaterialName);
            }

            var model = new ApplicationSubmissionConfirmationViewModel
            {
                ApplicationReferenceNumber = applicationReferenceNumber,
                SiteLocation = UkNation.England,    // hardcoded until site information is available
                MaterialName = accreditation.MaterialName.ToLower(),
            };

            return View(model);
        }

        [HttpGet(PagePaths.SelectOverseasSites, Name = RouteIds.SelectOverseasSites)]
        public async Task<IActionResult> SelectOverseasSites([FromRoute] Guid accreditationId)
        {
            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.ExporterAccreditationTaskList, new { AccreditationId = accreditationId });

            var model = new SelectOverseasSitesViewModel
            {                
                AccreditationId = accreditationId,
                OverseasSites = new List<SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd", Group = new SelectListGroup { Name = "France" } },
                    new() { Value = "2", Text = "DEF Exporters Ltd", Group = new SelectListGroup { Name = "Germany" } },
                    new() { Value = "3", Text = "GHI Exporters Ltd", Group = new SelectListGroup { Name = "Vietnam" } },
                    new() { Value = "4", Text = "JKL Exporters Ltd", Group = new SelectListGroup { Name = "Brazil" } },
                    new() { Value = "5", Text = "MNO Exporters Ltd", Group = new SelectListGroup { Name = "Canada" } },
                    new() { Value = "6", Text = "PQR Exporters Ltd", Group = new SelectListGroup { Name = "Australia" } },
                    new() { Value = "7", Text = "STU Exporters Ltd", Group = new SelectListGroup { Name = "Japan" } },
                    new() { Value = "8", Text = "VWX Exporters Ltd", Group = new SelectListGroup { Name = "South Africa" } },
                    new() { Value = "9", Text = "YZA Exporters Ltd", Group = new SelectListGroup { Name = "India" } },
                    new() { Value = "10", Text = "BCD Exporters Ltd", Group = new SelectListGroup { Name = "United States" } },
                    new() { Value = "11", Text = "EFG Exporters Ltd", Group = new SelectListGroup { Name = "Spain" } }
                }
            };
            return View(model);            
        }

        [HttpPost(PagePaths.SelectOverseasSites, Name = RouteIds.SelectOverseasSites)]
        public async Task<IActionResult> SelectOverseasSites(SelectOverseasSitesViewModel model)
        {
            return model.Action switch
            {
                "continue" => RedirectToRoute(RouteIds.CheckAnswersPERNs, new { model.AccreditationId }),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        private AccreditationRequestDto GetAccreditationRequestDto(AccreditationDto accreditation)
        {
            return new AccreditationRequestDto
            {
                ExternalId = accreditation.ExternalId,
                OrganisationId = accreditation.OrganisationId,
                RegistrationMaterialId = accreditation.RegistrationMaterialId,
                ApplicationTypeId = accreditation.ApplicationTypeId,
                AccreditationStatusId = accreditation.AccreditationStatusId,
                DecFullName = accreditation.DecFullName,
                DecJobTitle = accreditation.DecJobTitle,
                AccreferenceNumber = accreditation.AccreferenceNumber,
                AccreditationYear = accreditation.AccreditationYear,
                PrnTonnage = accreditation.PrnTonnage,
                PrnTonnageAndAuthoritiesConfirmed = accreditation.PrnTonnageAndAuthoritiesConfirmed,
                InfrastructurePercentage = accreditation.InfrastructurePercentage,
                PackagingWastePercentage = accreditation.PackagingWastePercentage,
                BusinessCollectionsPercentage = accreditation.BusinessCollectionsPercentage,
                NewUsesPercentage = accreditation.NewUsesPercentage,
                NewMarketsPercentage = accreditation.NewMarketsPercentage,
                CommunicationsPercentage = accreditation.CommunicationsPercentage,
                OtherPercentage = accreditation.OtherPercentage,
                InfrastructureNotes = accreditation.InfrastructureNotes,
                PackagingWasteNotes = accreditation.PackagingWasteNotes,
                BusinessCollectionsNotes = accreditation.BusinessCollectionsNotes,
                NewUsesNotes = accreditation.NewUsesNotes,
                NewMarketsNotes = accreditation.NewMarketsNotes,
                CommunicationsNotes = accreditation.CommunicationsNotes,
                OtherNotes = accreditation.OtherNotes,
                BusinessPlanConfirmed = accreditation.BusinessPlanConfirmed
            };
        }

        private string? GetBusinessPlanPercentage(decimal? businessPlanPercentage)
        {
            return businessPlanPercentage.HasValue ? ((int)businessPlanPercentage.Value).ToString() : null;
        }

        private decimal? GetBusinessPlanPercentage(string? businessPlanPercentage)
        {
            return !string.IsNullOrEmpty(businessPlanPercentage) ? decimal.Parse(businessPlanPercentage) : null;
        }

        private void SetBackLink(string previousPageRouteId, Guid? accreditationId)
        {
            var routeValues = accreditationId != null ? new { accreditationId } : null;
            ViewBag.BackLinkToDisplay = Url.RouteUrl(previousPageRouteId, routeValues);
        }

        private string GetSubject(string prnRouteName)
        {
            return HttpContext.GetRouteName() == prnRouteName ? "PRN" : "PERN";
        }

        static string[] pernRouteNames =
                [
                    RouteIds.SelectAuthorityPERNs,
                    RouteIds.CheckAnswersPERNs,
                    RouteIds.MoreDetailOnBusinessPlanPERNs,
                    RouteIds.CheckBusinessPlanPERN,
                    RouteIds.SelectPernTonnage,
                    RouteIds.ExporterAccreditationTaskList

                ];
        private void ValidateRouteForApplicationType(ApplicationType applicationType)
        {
            
            var isPERNRoute = pernRouteNames.Contains(HttpContext.GetRouteName());

            if (!isPERNRoute && applicationType == ApplicationType.Exporter )
                throw new InvalidOperationException("A PRN route name can not be used for an Exporter accreditation.");
            if (isPERNRoute && applicationType == ApplicationType.Reprocessor)
                throw new InvalidOperationException("A PERN route name can not be used for a Reprocessor accreditation.");
        }

        private static TaskStatus GetTonnageAndAuthorityToIssuePrnStatus(
            int? prnTonnage,
            bool prnTonnageAndAuthoritiesConfirmed,
            List<AccreditationPrnIssueAuthDto> authorisedUsers)
        {
            if (prnTonnageAndAuthoritiesConfirmed && prnTonnage.HasValue && authorisedUsers?.Any() == true)
            {
                return App.Enums.TaskStatus.Completed;
            }
            else if (prnTonnage.HasValue || authorisedUsers?.Any() == true )
            {
                return TaskStatus.InProgress;
            }
            else
            {
                return TaskStatus.NotStart;
            }
        }

        private static TaskStatus GetBusinessPlanStatus(AccreditationDto? accreditation)
        {
            if (accreditation.BusinessPlanConfirmed)
                return TaskStatus.Completed;

            // if all percentages are null, then status is NotStart.
            if (accreditation.InfrastructurePercentage == null &&
                    accreditation.PackagingWastePercentage == null &&
                    accreditation.BusinessCollectionsPercentage == null &&
                    accreditation.CommunicationsPercentage == null &&
                    accreditation.NewMarketsPercentage == null &&
                    accreditation.NewUsesPercentage == null &&
                    accreditation.OtherPercentage == null)
                return TaskStatus.NotStart;
                        
            return TaskStatus.InProgress;
        }

        private static TaskStatus GetAccreditationSamplingAndInspectionPlanStatus(bool isFileUploadSimulated)
        {
            if (isFileUploadSimulated)
            {
                return TaskStatus.Completed;
            }

            return TaskStatus.NotStart;
        }
    }
}
