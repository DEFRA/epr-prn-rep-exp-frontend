using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

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
    }
}
