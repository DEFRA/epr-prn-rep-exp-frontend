using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

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
    }
}
