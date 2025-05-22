using Epr.Reprocessor.Exporter.UI.App.Constants;
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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;

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
            public const string BusinessPlanPercentages = "accreditation.busines-plan-percentages";
            public const string ApplyingFor2026Accreditation = "accreditation.applying-for-2026-accreditation";
        }

        [HttpGet(PagePaths.ApplicationSaved, Name = RouteIds.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["ApplicationTitle"] = sharedLocalizer["application_title_accreditation"];
            base.OnActionExecuting(context);
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
                1 =>  RedirectToRoute(RouteIds.AccreditationTaskList, new { accreditationId }),
                2 =>  RedirectToRoute(RouteIds.ExporterAccreditationTaskList, new { accreditationId }),
                _ => BadRequest("Invalid application type supplied.")
            };
        }

        [HttpGet, Route(PagePaths.NotAnApprovedPerson)]
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
                    RedirectToRoute(RouteIds.SelectAuthorityPRNs, new { model.AccreditationId }):
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

            model.Subject = HttpContext.GetRouteName() == RouteIds.SelectAuthorityPRNs ? "PRN" : "PERN";

            ViewBag.BackLinkToDisplay = Url.RouteUrl(
                routeName: (model.Subject == "PERN" ? RouteIds.SelectPernTonnage : RouteIds.SelectPrnTonnage),
                values: new { accreditationId = accreditationId });
         
            var authorisedUsers = await accreditationService.GetAccreditationPrnIssueAuths(accreditationId);

            model.SelectedAuthorities = authorisedUsers?.Select(x => x.PersonExternalId.ToString()).ToList() ?? new List<string>();
            var userData = User.GetUserData();

            List<ManageUserDto> users = new();
            
            users.AddRange(await accreditationService.GetOrganisationUsers(userData,true));            

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

            await accreditationService.ReplaceAccreditationPrnIssueAuths(model.AccreditationId, requestDtos);

            return model.Action switch
            {
                "continue" => model.Subject == "PERN" ? RedirectToRoute(RouteIds.CheckAnswersPERNs, new { model.AccreditationId }) : RedirectToRoute(RouteIds.CheckAnswersPRNs, new { model.AccreditationId }),
                //"save" => BadRequest("Invalid action supplied: save."),
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

            var model = new CheckAnswersViewModel
            {
                AccreditationId = accreditationId,
                PrnTonnage = accreditation?.PrnTonnage,
                AuthorisedUsers = authorisedSelectedUsers != null ? string.Join(", ", authorisedSelectedUsers) : string.Empty,
                Subject = subject,
                TonnageChangeRoutePath = subject == "PRN" ? RouteIds.SelectPrnTonnage : RouteIds.SelectPernTonnage,
                AuthorisedUserChangeRoutePath = subject == "PRN" ? RouteIds.SelectAuthorityPRNs : RouteIds.SelectAuthorityPERNs,
            };

            SetBackLink(model.Subject == "PRN" ? RouteIds.SelectAuthorityPRNs : RouteIds.SelectAuthorityPERNs, model.AccreditationId);

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

        [HttpGet(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinessPlanPercentages)]
        public async Task<IActionResult> BusinessPlan(Guid accreditationId)
        {
            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.AccreditationTaskList, new { AccreditationId = accreditationId });

            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            var model = new BusinessPlanViewModel()
            {
                ExternalId = accreditation.ExternalId,
                MaterialName = accreditation.MaterialName,
                Subject = "PRN",
                InfrastructurePercentage = (int)accreditation.InfrastructurePercentage.GetValueOrDefault(0),
                PackagingWastePercentage = (int)accreditation.PackagingWastePercentage.GetValueOrDefault(0),
                BusinessCollectionsPercentage = (int)accreditation.BusinessCollectionsPercentage.GetValueOrDefault(0),
                CommunicationsPercentage = (int)accreditation.CommunicationsPercentage.GetValueOrDefault(0),
                NewMarketsPercentage = (int)accreditation.NewMarketsPercentage.GetValueOrDefault(0),
                NewUsesPercentage = (int)accreditation.NewUsesPercentage.GetValueOrDefault(0),
            };

            return View(model);
        }

        [HttpPost(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinessPlanPercentages)]
        public async Task<IActionResult> BusinessPlan(BusinessPlanViewModel model)
        {
            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.AccreditationTaskList, new { AccreditationId = model.ExternalId });

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.ExternalId);
            var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
            accreditationRequestDto.BusinessCollectionsPercentage = model.BusinessCollectionsPercentage;
            accreditationRequestDto.CommunicationsPercentage = model.CommunicationsPercentage;
            accreditationRequestDto.InfrastructurePercentage = model.InfrastructurePercentage;
            accreditationRequestDto.NewMarketsPercentage = model.NewMarketsPercentage;
            accreditationRequestDto.NewUsesPercentage = model.NewUsesPercentage;
            accreditationRequestDto.PackagingWastePercentage = model.PackagingWastePercentage;

            await accreditationService.UpsertAccreditation(accreditationRequestDto);

            // Navigate to next page
            switch (model.Action)
            {
                case "continue":
                    {
                        return RedirectToRoute(RouteIds.MoreDetailOnBusinessPlanPRNs, new { accreditationId = model.ExternalId });
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
            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.BusinessPlanPercentages, new { AccreditationId = accreditationId });

            var accreditation = await accreditationService.GetAccreditation(accreditationId);

            var model = new MoreDetailOnBusinessPlanViewModel()
            {
                ExternalId = accreditation.ExternalId,
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
                Subject = HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ? "PRN" : "PERN",
                FormPostRouteName = HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ?
                    AccreditationController.RouteIds.MoreDetailOnBusinessPlanPRNs :
                    AccreditationController.RouteIds.MoreDetailOnBusinessPlanPERNs
            };

            return View(model);
        }

        [HttpPost(PagePaths.MoreDetailOnBusinessPlanPRNs, Name = RouteIds.MoreDetailOnBusinessPlanPRNs),
            HttpPost(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan(MoreDetailOnBusinessPlanViewModel model)
        {
            ViewBag.BackLinkToDisplay = Url.RouteUrl(RouteIds.BusinessPlanPercentages, new { AccreditationId = model.ExternalId });

            if (!ModelState.IsValid)
            {
                model.Subject = HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ? "PRN" : "PERN";
                return View(model);
            }

            var accreditation = await accreditationService.GetAccreditation(model.ExternalId);
            var accreditationRequestDto = GetAccreditationRequestDto(accreditation);
            accreditationRequestDto.InfrastructureNotes = accreditation.InfrastructurePercentage > 0 ? model.Infrastructure : "";
            accreditationRequestDto.PackagingWasteNotes = accreditation.PackagingWastePercentage > 0 ? model.PriceSupport : "";
            accreditationRequestDto.BusinessCollectionsNotes = accreditation.BusinessCollectionsPercentage > 0 ? model.BusinessCollections : "";
            accreditationRequestDto.CommunicationsNotes = accreditation.CommunicationsPercentage > 0 ? model.Communications : "";
            accreditationRequestDto.NewMarketsNotes = accreditation.NewMarketsPercentage > 0 ? model.NewMarkets : "";
            accreditationRequestDto.NewUsesNotes = accreditation.NewUsesPercentage > 0 ? model.NewUses : "";

            await accreditationService.UpsertAccreditation(accreditationRequestDto);

            return model.Action switch
            {
                "continue" => RedirectToRoute(HttpContext.GetRouteName() == RouteIds.MoreDetailOnBusinessPlanPRNs ? RouteIds.CheckAnswersPRNs : RouteIds.CheckAnswersPERNs, new { accreditationId = model.ExternalId }),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(template: PagePaths.ApplyForAccreditation, Name = PagePaths.ApplyForAccreditation)]
        public IActionResult ApplyforAccreditation() => View(new ApplyForAccreditationViewModel());


        [HttpGet(PagePaths.AccreditationTaskList, Name = RouteIds.AccreditationTaskList), HttpGet(PagePaths.ExporterAccreditationTaskList, Name = RouteIds.ExporterAccreditationTaskList)]
        public async Task<IActionResult> TaskList()
        {
            var userData = User.GetUserData();
            var organisationId = userData.Organisations[0].Id.ToString();
            var approvedPersons = new List<string>();

            var isAuthorisedUser = userData.ServiceRoleId == (int)ServiceRole.Approved || userData.ServiceRoleId == (int)ServiceRole.Delegated;
            if (!isAuthorisedUser)
            {
                var usersApproved = await accountServiceApiClient.GetUsersForOrganisationAsync(organisationId, (int)ServiceRole.Approved);

                foreach (var user in usersApproved)
                {
                    approvedPersons.Add($"{user.FirstName} {user.LastName}");
                }
            }
            var viewModel = new SubmitAccreditationApplicationViewModel
            {
                IsApprovedUser = isAuthorisedUser,
                PeopleCanSubmitApplication = new PeopleAbleToSubmitApplication { ApprovedPersons = approvedPersons }
            };
            return View(viewModel);
        }


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

        [HttpGet(PagePaths.ApplyingFor2026Accreditation, Name = RouteIds.ApplyingFor2026Accreditation)]
        public IActionResult ApplyingFor2026Accreditation()
        {
            /*
             *  As per figma workflow on 21/5/2025 the previous pages in the worflow are :
             *  if user is authorised person then 
             *      accreditation/reprocessor/multiple
             *  else
             *      accreditation/authorised-signatory
             *      
             *  When these pages are available look up if the user is authorised and call SetBackLink based on the result
             */


            ViewBag.BackLinkToDisplay = "#";


            return View();
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

        private string GetSubject(string prnRouteName)
        {
            return HttpContext.GetRouteName() == prnRouteName ? "PRN" : "PERN";
        }
    }
}
