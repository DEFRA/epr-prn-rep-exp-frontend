using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Microsoft.AspNetCore.Mvc.Routing;

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
        }

        [HttpGet(PagePaths.ApplicationSaved, Name = RouteIds.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["ApplicationTitle"] = sharedLocalizer["application_title_accreditation"];
            base.OnActionExecuting(context);
        }

        [HttpGet, Route(PagePaths.NotAnApprovedPerson)]
        public async Task<IActionResult> NotAnApprovedPerson()
        {
            var userData = User.GetUserData();
            var organisationId = userData.Organisations[0].Id.ToString();

            var usersApproved = accountServiceApiClient.GetUsersForOrganisationAsync(organisationId, (int)ServiceRole.Approved).Result.ToList();
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

            // Get accreditation from facade:
            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            // Only use the properties we need:
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
                    new { AccreditationId = model.AccreditationId});

                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.AccreditationId);
            accreditation.PrnTonnage = model.PrnTonnage;

            var request = GetAccreditationRequestDto(accreditation);
            await accreditationService.UpsertAccreditation(request);

            return model.Action switch
            {
                "continue" => RedirectToRoute(RouteIds.SelectAuthorityPRNs, new { model.AccreditationId }), // Will be finalised in future navigation story.
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }


        [HttpGet(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
            HttpGet(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]
        public async Task<IActionResult> SelectAuthority([FromRoute] Guid accreditationId)
        {
            var model = new SelectAuthorityViewModel();

            var userData = User.GetUserData();

            var users = await accreditationService.GetOrganisationUsers(userData);


            model.Authorities.AddRange(users.Select(x => new SelectListItem
            {
                Value = x.PersonId.ToString(),
                Text = x.FirstName + " " + x.LastName,
                Group = new SelectListGroup { Name = x.Email }
            }
                ).ToList());


            // When the backend data is available get the site address and selected authorities and map them to the model.



            model.Subject = HttpContext.GetRouteName() == RouteIds.SelectAuthorityPRNs ? "PRN" : "PERN";


            return View(model);
        }


        [HttpPost(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
            HttpPost(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]

        public async Task<IActionResult> SelectAuthority(SelectAuthorityViewModel model)
        {
            var validationResult = await validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            return model.Action switch
            {
                "continue" => model.Subject == "PERN" ? RedirectToRoute(RouteIds.CheckAnswersPERNs, new { model.AccreditationId }) : RedirectToRoute(RouteIds.CheckAnswersPRNs, new { model.AccreditationId }),
                //"save" => BadRequest("Invalid action supplied: save."),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs), HttpGet(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs)]
        public async Task<IActionResult> CheckAnswers([FromRoute] Guid accreditationId)
        {
            // Get accreditation object
            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            // Get selected users to issue prns
            var prnIssueAuths = await accreditationService.GetAccreditationPrnIssueAuths(accreditationId);

            // Get organisation users
            var users = await accreditationService.GetOrganisationUsers(User.GetUserData());

            var authPersonIds = prnIssueAuths?.Select(a => a.ExternalId).ToHashSet();

            var authorisedSelectedUsers = users != null && authPersonIds != null
                ? users
                    .Where(u => authPersonIds.Contains(u.PersonId))
                    .Select(u => u.FirstName + " " + u.LastName)
                : null;

            var model = new CheckAnswersViewModel
            {
                AccreditationId = accreditationId,
                PrnTonnage = accreditation?.PrnTonnage,
                AuthorisedUsers = authorisedSelectedUsers != null ? string.Join(", ", authorisedSelectedUsers) : string.Empty,
            };

            SetBackLink(RouteIds.SelectAuthorityPRNs, model.AccreditationId);

            ViewBag.Subject = HttpContext.GetRouteName() == RouteIds.CheckAnswersPRNs ? "PRN" : "PERN";

            return View(model);
        }

        [HttpPost(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs),
            HttpPost(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs)]
        public async Task<IActionResult> CheckAnswers(CheckAnswersViewModel model)
        {
            return model.Action switch
            {
                "continue" => model.Subject == "PERN" ? RedirectToRoute(RouteIds.ExporterAccreditationTaskList) : RedirectToRoute(RouteIds.AccreditationTaskList),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

            [HttpGet(PagePaths.BusinessPlan)]
        public async Task<IActionResult> BusinessPlan() => View(new BusinessPlanViewModel());

        [HttpGet(PagePaths.MoreDetailOnBusinessPlanPRNs, Name = RouteIds.MoreDetailOnBusinessPlanPRNs),
            HttpGet(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var model = new MoreDetailOnBusinessPlanViewModel()
            {
                ShowInfrastructure = true,
                ShowPriceSupport = true,
                ShowBusinessCollections = true,
                ShowCommunications = true,
                ShowNewMarkets = true,
                ShowNewUses = true,
                Subject = HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ? "PRN" : "PERN"
            };

            return View(model);
        }

        [HttpPost(PagePaths.MoreDetailOnBusinessPlanPRNs, Name = RouteIds.MoreDetailOnBusinessPlanPRNs),
            HttpPost(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan(MoreDetailOnBusinessPlanViewModel model)
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            if (!ModelState.IsValid)
            {
                model.Subject = HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ? "PRN" : "PERN";
                return View(model);
            }

            // Save logic TBC.

            return model.Action switch
            {
                "continue" => RedirectToRoute(RouteIds.MoreDetailOnBusinessPlanPRNs), // Will be finalised in future navigation story.
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(template: PagePaths.ApplyForAccreditation, Name = PagePaths.ApplyForAccreditation)]
        public IActionResult ApplyforAccreditation() => View(new ApplyForAccreditationViewModel());


        [HttpGet(PagePaths.AccreditationTaskList, Name = RouteIds.AccreditationTaskList), HttpGet(PagePaths.ExporterAccreditationTaskList, Name = RouteIds.ExporterAccreditationTaskList)]
        public async Task<IActionResult> TaskList() => View();


        [HttpGet(PagePaths.CheckBusinessPlanPRN, Name = RouteIds.CheckBusinessPlanPRN), HttpGet(PagePaths.CheckBusinessPlanPERN, Name = RouteIds.CheckBusinessPlanPERN)]
        public IActionResult ReviewBusinessPlan()
        {
            const string emptyNotesContent = "None provided";
            var model = new ReviewBusinessPlanViewModel();
            model.InfrastructureNotes = "To achieve operational capacity by investing in new machinery";
            model.InfrastructurePercentage = 55;

            model.PriceSupportNotes = "To competetivley price our service";
            model.PriceSupportPercentage = 5;

            model.BusinessCollectionsNotes = emptyNotesContent;
            model.BusinessCollectionsPercentage = 10;

            model.CommunicationsNotes = emptyNotesContent;
            model.CommunicationsPercentage = 2;

            model.DevelopingMarketsNotes = emptyNotesContent;
            model.DevelopingMarketsPercentage = 15;

            model.DevelopingNewUsesNotes = emptyNotesContent;
            model.DevelopingNewUsesPercentage = 10;


            ViewBag.Subject = HttpContext.GetRouteName() == RouteIds.CheckBusinessPlanPRN ? "PRN" : "PERN";

            return View(model);
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
                InfrastructurePercentage = accreditation.InfrastructurePercentage,
                PackagingWastePercentage = accreditation.PackagingWastePercentage,
                BusinessCollectionsPercentage = accreditation.BusinessCollectionsPercentage,
                NewUsesPercentage = accreditation.NewUsesPercentage,
                NewMarketsPercentage = accreditation.NewMarketsPercentage,
                CommunicationsPercentage = accreditation.CommunicationsPercentage,
                InfrastructureNotes = accreditation.InfrastructureNotes,
                PackagingWasteNotes = accreditation.PackagingWasteNotes,
                BusinessCollectionsNotes = accreditation.BusinessCollectionsNotes,
                NewUsesNotes = accreditation.NewUsesNotes,
                NewMarketsNotes = accreditation.NewMarketsNotes,
                CommunicationsNotes = accreditation.CommunicationsNotes,
            };
        }

        private void SetBackLink(string previousPageRouteId, Guid? accreditationId)
        {
            var routeValues = accreditationId != null ? new { accreditationId } : null;
            ViewBag.BackLinkToDisplay = Url.RouteUrl(previousPageRouteId, routeValues);
        }
    }
}
