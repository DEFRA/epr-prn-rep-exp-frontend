using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Microsoft.CodeAnalysis.CodeActions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using EPR.Common.Authorization.Models;
using Epr.Reprocessor.Exporter.UI.Extensions;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.AccreditationLanding)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public class AccreditationController(IStringLocalizer<SharedResources> sharedLocalizer,
        IOptions<ExternalUrlOptions> externalUrlOptions,
        IValidationService validationService,
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
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var viewModel = new NotAnApprovedPersonViewModel()
            {
                ApprovedPersons = new List<string>
                {
                    "Andrew Recycler",
                    "Gary Packaging",
                    "Scott Reprocessor Recycler"
                }
            };

            return View(viewModel);
        }

        [HttpGet(PagePaths.CalendarYear)]
        public IActionResult CalendarYear() => View(new CalendarYearViewModel { NpwdLink = externalUrlOptions.Value.NationalPackagingWasteDatabase });

        [HttpGet(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage), HttpGet(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage)]
        public async Task<IActionResult> PrnTonnage()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var model = new PrnTonnageViewModel()
            {
                MaterialName = "steel",
                Subject = HttpContext.GetRouteName() == RouteIds.SelectPrnTonnage ? "PRN" : "PERN"
            };

            return View(model);
        }

        [HttpPost(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage), HttpPost(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage)]
        public async Task<IActionResult> PrnTonnage(PrnTonnageViewModel model)
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Save logic TBC.

            return model.Action switch
            {
                "continue" => RedirectToRoute(RouteIds.SelectPrnTonnage), // Will be finalised in future navigation story.
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }


        [HttpGet(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs), HttpGet(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]
        public async Task<IActionResult> SelectAuthority()
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


        [ValidateAntiForgeryToken, HttpPost(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
            HttpPost(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs)]
        public async Task<IActionResult> SelectAuthority(SelectAuthorityViewModel model)
        {
            var validationResult = await validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(model);
            }

            model.Subject = HttpContext.GetRouteName() == RouteIds.SelectAuthorityPRNs ? "PRN" : "PERN";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return model.Action switch
            {
                "continue" => model.Subject == "PERN" ? RedirectToRoute(RouteIds.CheckAnswersPERNs) : RedirectToRoute(RouteIds.CheckAnswersPRNs),
                //"save" => BadRequest("Invalid action supplied: save."),
                "save" => RedirectToRoute(RouteIds.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs), HttpGet(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs)]
        public IActionResult CheckAnswers()
        {            
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.
            ViewBag.Subject = HttpContext.GetRouteName() == RouteIds.CheckAnswersPRNs ? "PRN" : "PERN";

            return View();
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


        [HttpGet(PagePaths.AccreditationTaskList), HttpGet(PagePaths.ExporterAccreditationTaskList)] 
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
    }
}
