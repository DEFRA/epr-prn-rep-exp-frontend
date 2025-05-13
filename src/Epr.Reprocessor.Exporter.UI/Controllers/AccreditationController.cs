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
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[ExcludeFromCodeCoverage]
[Route(PagePaths.AccreditationLanding)]
[FeatureGate(FeatureFlags.ShowAccreditation)]
public class AccreditationController(
    IStringLocalizer<SharedResources> sharedLocalizer,
    IOptions<ExternalUrlOptions> externalUrlOptions,
    IAccreditationService accreditationService,
    ILogger<AccreditationController> logger) : Controller
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
        public const string BusinesPlanPercentages = "accreditation.busines-plan-percentages";
    }

    [HttpGet(PagePaths.ApplicationSaved, Name = RouteIds.ApplicationSaved)]
    public IActionResult ApplicationSaved() => View();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["ApplicationTitle"] = sharedLocalizer["application_title_accreditation"];
        base.OnActionExecuting(context);
    }

    [HttpGet]
    [Route(PagePaths.NotAnApprovedPerson)]
    [FeatureGate(FeatureFlags.ShowNotAnApprovedPerson)]
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

    [HttpGet(PagePaths.CalendarYear), FeatureGate(FeatureFlags.ShowCalendarYear)]
    public IActionResult CalendarYear() => View(new CalendarYearViewModel { NpwdLink = externalUrlOptions.Value.NationalPackagingWasteDatabase });

    [HttpGet(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage),
        HttpGet(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage),
        FeatureGate(FeatureFlags.ShowPrnTonnage)]
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

    [HttpPost(PagePaths.SelectPrnTonnage, Name = RouteIds.SelectPrnTonnage),
        HttpPost(PagePaths.SelectPernTonnage, Name = RouteIds.SelectPernTonnage),
        FeatureGate(FeatureFlags.ShowPrnTonnage)]
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


    [HttpGet(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
        HttpGet(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs),
        FeatureGate(FeatureFlags.ShowSelectAuthority)]
    public async Task<IActionResult> SelectAuthority()
    {
        var model = new SelectAuthorityViewModel();
        model.Subject = HttpContext.GetRouteName() == RouteIds.SelectAuthorityPRNs ? "PRN" : "PERN";


        model.Authorities.AddRange([
             new SelectListItem { Value = "myself", Text = "Myself", Group = new SelectListGroup { Name = "Myself@reprocessor.com" } },
                new SelectListItem { Value = "andrew", Text = "Andrew Recycler", Group = new SelectListGroup { Name = "Andrew.Recycler@reprocessor.com" } },
                new SelectListItem { Value = "gary1", Text = "Gary Package", Group = new SelectListGroup { Name = "Gary.Package1@reprocessor.com" } },
                new SelectListItem { Value = "gary2", Text = "Gary Package", Group = new SelectListGroup { Name = "GaryWPackageP@reprocessor.com" } },
                new SelectListItem { Value = "scott", Text = "Scott Reprocessor", Group = new SelectListGroup { Name = "Scott.Reprocessor@reprocessor.com" } }
                   ]);

        return View(model);
    }


    [ValidateAntiForgeryToken]
    [HttpPost(PagePaths.SelectAuthorityPRNs, Name = RouteIds.SelectAuthorityPRNs),
        HttpPost(PagePaths.SelectAuthorityPERNs, Name = RouteIds.SelectAuthorityPERNs),
        FeatureGate(FeatureFlags.ShowSelectAuthority)]
    public async Task<IActionResult> SelectAuthority(SelectAuthorityViewModel model)
    {
        model.Subject = HttpContext.GetRouteName() == RouteIds.SelectAuthorityPRNs ? "PRN" : "PERN";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return model.Action switch
        {
            "continue" => BadRequest("Invalid action supplied: continue."),
            //"save" => BadRequest("Invalid action supplied: save."),
            "save" => RedirectToRoute(RouteIds.ApplicationSaved),
            _ => BadRequest("Invalid action supplied.")
        };
    }

    [HttpGet(PagePaths.CheckAnswersPRNs, Name = RouteIds.CheckAnswersPRNs),
        HttpGet(PagePaths.CheckAnswersPERNs, Name = RouteIds.CheckAnswersPERNs),
        FeatureGate(FeatureFlags.ShowCheckAnswers)]
    public IActionResult CheckAnswers()
    {
        ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.
        ViewBag.Subject = HttpContext.GetRouteName() == RouteIds.CheckAnswersPRNs ? "PRN" : "PERN";

        return View();
    }

    [HttpGet(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinesPlanPercentages)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public async Task<IActionResult> BusinessPlan()
    {
        var model = new BusinessPlanViewModel()
        {
            MaterialName = "steel",
            InfrastructurePercentage = 55,
            PackagingWastePercentage = 5,
            BusinessCollectionsPercentage = 10,
            CommunicationsPercentage = 2,
            NewMarketsPercentage = 15,
            NewUsesPercentage = 10
        };

        return View(model);
    }

    [HttpPost(PagePaths.BusinessPlanPercentages, Name = RouteIds.BusinesPlanPercentages),
        FeatureGate(FeatureFlags.ShowAccreditation)]
    public async Task<IActionResult> BusinessPlan(BusinessPlanViewModel model)
    {
        ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Save to API
        var id = await accreditationService.AddAsync(new AccreditationRequestDto
        {
            // Map model to DTO
            BusinessCollectionsPercentage = model.BusinessCollectionsPercentage,
            InfrastructurePercentage = model.InfrastructurePercentage,
            NewMarketsPercentage = model.NewMarketsPercentage,
            NewUsesPercentage = model.NewUsesPercentage,
            PackagingWastePercentage = model.PackagingWastePercentage,
            CommunicationsPercentage = model.CommunicationsPercentage,
        });

        // Navigate to next page
        switch (model.Action)
        {
            case "continue":
                {                    
                    return RedirectToRoute(RouteIds.MoreDetailOnBusinessPlanPRNs);
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
        HttpGet(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs),
        FeatureGate(FeatureFlags.ShowMoreDetailOnBusinessPlan)]
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
        HttpPost(PagePaths.MoreDetailOnBusinessPlanPERNs, Name = RouteIds.MoreDetailOnBusinessPlanPERNs),
        FeatureGate(FeatureFlags.ShowMoreDetailOnBusinessPlan)]
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

    [HttpGet(template: PagePaths.ApplyForAccreditation, Name = PagePaths.ApplyForAccreditation), FeatureGate(FeatureFlags.ShowApplyForAccreditation)]
    public IActionResult ApplyforAccreditation() => View(new ApplyForAccreditationViewModel());


    [HttpGet(PagePaths.AccreditationTaskList), FeatureGate(FeatureFlags.ShowAccreditationTaskList)]
    [HttpGet(PagePaths.ExporterAccreditationTaskList), FeatureGate(FeatureFlags.ShowExporterAccreditationTaskList)]
    public async Task<IActionResult> TaskList() => View();


    [HttpGet(PagePaths.CheckBusinessPlanPRN, Name = RouteIds.CheckBusinessPlanPRN),
        HttpGet(PagePaths.CheckBusinessPlanPERN, Name = RouteIds.CheckBusinessPlanPERN),
        FeatureGate(FeatureFlags.ShowCheckBusinessPlan)]
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

    [HttpGet(PagePaths.AccreditationSamplingAndInspectionPlan), FeatureGate(FeatureFlags.ShowAccreditationSamplingAndInspectionPlan)]
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
