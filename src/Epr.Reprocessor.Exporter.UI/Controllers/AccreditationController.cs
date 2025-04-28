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

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.AccreditationLanding)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public class AccreditationController(IStringLocalizer<SharedResources> sharedLocalizer) : Controller
    {
        public static class RouteIds
        {
            public const string SelectAuthorityPRNs = "accreditation.select-authority-for-people-prns";
            public const string SelectAuthorityPERNs = "accreditation.select-authority-for-people-perns";
            public const string ApplicationSaved = "accreditation.application-saved";
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

        [HttpGet]
        [Route(PagePaths.SelectMaterial)]
        [FeatureGate(FeatureFlags.ShowSelectMaterial)]
        public async Task<IActionResult> SelectMaterial()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var viewModel = new SelectMaterialViewModel()
            {
                Materials = new List<SelectListItem>
                {
                    new SelectListItem("Steel (R4)", "steel"),
                    new SelectListItem("Wood (R3)", "wood")
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route(PagePaths.SelectMaterial)]
        [FeatureGate(FeatureFlags.ShowSelectMaterial)]
        public async Task<IActionResult> SelectMaterial(SelectMaterialViewModel viewModel, string action)
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Save logic TBC.

            return Redirect(PagePaths.SelectMaterial); // Will be finalised in future navigation story.
        }

        [HttpGet]
        [Route(PagePaths.SelectPrnTonnage)]
        [FeatureGate(FeatureFlags.ShowPrnTonnage)]
        public async Task<IActionResult> PrnTonnage()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var viewModel = new PrnTonnageViewModel()
            {
                MaterialName = "steel"
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route(PagePaths.SelectPrnTonnage)]
        [FeatureGate(FeatureFlags.ShowPrnTonnage)]
        public async Task<IActionResult> PrnTonnage(PrnTonnageViewModel viewModel, string action)
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Save logic TBC.

            return action switch
            {
                "continue" => Redirect(PagePaths.SelectPrnTonnage), // Will be finalised in future navigation story.
                "save" => Redirect(PagePaths.ApplicationSaved),
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

            model.Authorities.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "paul", Text = "Paul Reprocessor", Group = new SelectListGroup { Name = "paul.Reprocessor@reprocessor.com" } });
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

        [HttpGet(PagePaths.CheckAnswers), FeatureGate(FeatureFlags.ShowCheckAnswers)]
        public IActionResult CheckAnswers() => View();

        [HttpGet(PagePaths.BusinessPlan), FeatureGate(FeatureFlags.ShowBusinessPlan)]
        public async Task<IActionResult> BusinessPlan() => View(new BusinessPlanViewModel());

        [HttpGet]
        [Route(PagePaths.MoreDetailOnBusinessPlan)]
        [FeatureGate(FeatureFlags.ShowMoreDetailOnBusinessPlan)]

        public async Task<IActionResult> MoreDetailOnBusinessPlan()
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            var viewModel = new MoreDetailOnBusinessPlanViewModel()
            {
                ShowInfrastructure = true,
                ShowPriceSupport = true,
                ShowBusinessCollections = true,
                ShowCommunications = true,
                ShowNewMarkets = true,
                ShowNewUses = true,
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route(PagePaths.MoreDetailOnBusinessPlan)]
        [FeatureGate(FeatureFlags.ShowMoreDetailOnBusinessPlan)]
        public async Task<IActionResult> MoreDetailOnBusinessPlan(MoreDetailOnBusinessPlanViewModel viewModel, string action)
        {
            ViewBag.BackLinkToDisplay = "#"; // Will be finalised in future navigation story.

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Save logic TBC.

            return action switch
            {
                "continue" => Redirect(PagePaths.MoreDetailOnBusinessPlan), // Will be finalised in future navigation story.
                "save" => Redirect(PagePaths.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };
        }

        [HttpGet(template: PagePaths.ApplyForAccreditation, Name = PagePaths.ApplyForAccreditation), FeatureGate(FeatureFlags.ShowApplyForAccreditation)]
        public IActionResult ApplyforAccreditation() => View(new ApplyForAccreditationViewModel());


        [HttpGet(PagePaths.AccreditationTaskList), FeatureGate(FeatureFlags.ShowAccreditationTaskList)]
        [HttpGet(PagePaths.ExporterAccreditationTaskList), FeatureGate(FeatureFlags.ShowExporterAccreditationTaskList)] 
        public async Task<IActionResult> TaskList() => View();

        
        [HttpGet(PagePaths.CheckBusinessPlan), FeatureGate(FeatureFlags.ShowCheckBusinessPlan)]
        public IActionResult ReviewBusinessPlan()
        {
            var model = new ReviewBusinessPlanViewModel();
            model.InfrastructureNotes = "Notes 1";
            model.InfrastructurePercentage = 50;

            model.PriceSupportNotes = "Notes 2";
            model.PriceSupportPercentage = 40;

            model.BusinessCollectionsNotes = "Notes 3";
            model.BusinessCollectionsPercentage = 10;

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
                        DateUploaded = DateTime.Now,
                        UploadedBy = "Jane Winston"
                    }
                }
            };

            return View(viewModel);
        }
    }
}
