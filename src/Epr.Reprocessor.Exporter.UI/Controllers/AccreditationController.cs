using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.AccreditationLanding)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public class AccreditationController(IStringLocalizer<SharedResources> sharedLocalizer) : Controller
    {
        [HttpGet(PagePaths.ApplicationSaved)]
        public IActionResult ApplicationSaved() => View();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["ApplicationTitle"] = sharedLocalizer["application_title_accreditation"];
            base.OnActionExecuting(context);
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
    

        [HttpGet]
        [Route(template: PagePaths.SelectAuthority, Name = PagePaths.SelectAuthority)]
        [FeatureGate(FeatureFlags.ShowSelectAuthority)]
        public async Task<IActionResult> SelectAuthority()
        {
            var model = new SelectAuthorityModel();
            model.Authorities.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "paul", Text = "Paul Reprocessor", Group = new SelectListGroup { Name = "paul.Reprocessor@reprocessor.com" } });
            model.Authorities.AddRange([
                 new SelectListItem { Value = "myself", Text = "Myself", Group = new SelectListGroup { Name = "Myself@reprocessor.com" } },
                    new SelectListItem { Value = "andrew", Text = "Andrew Recycler", Group = new SelectListGroup { Name = "Andrew.Recycler@reprocessor.com" } },
                    new SelectListItem { Value = "gary1", Text = "Gary Package", Group = new SelectListGroup { Name = "Gary.Package1@reprocessor.com" } },
                    new SelectListItem { Value = "gary2", Text = "Gary Package", Group = new SelectListGroup { Name = "GaryWPackageP@reprocessor.com" } },
                    new SelectListItem { Value = "scott", Text = "Scott Reprocessor", Group = new SelectListGroup { Name = "Scott.Reprocessor@reprocessor.com" } }
                       ]);

            await Task.CompletedTask; // Added to make the method truly async
            return View(model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route(template: PagePaths.SelectAuthority, Name = PagePaths.SelectAuthority)]
        [FeatureGate(FeatureFlags.ShowSelectAuthority)]
        public async Task<IActionResult> SelectAuthority(
            SelectAuthorityModel model,
            string action)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await Task.CompletedTask; // Added to make the method truly async

            return action switch
            {
                "continue" => BadRequest("Invalid action supplied: continue."),
                //"save" => BadRequest("Invalid action supplied: save."),
                "save" => Redirect(PagePaths.ApplicationSaved),
                _ => BadRequest("Invalid action supplied.")
            };


        }


        [HttpGet(PagePaths.CheckAnswers), FeatureGate(FeatureFlags.ShowCheckAnswers)]
        public IActionResult CheckAnswers() => View();

        [HttpGet(PagePaths.BusinessPlan), FeatureGate(FeatureFlags.ShowBusinessPlan)]
        public async Task<IActionResult> BusinessPlan() => View(new BusinessPlanViewModel());
    }
}
