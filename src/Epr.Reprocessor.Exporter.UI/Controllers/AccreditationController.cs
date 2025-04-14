using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    
    [Route(PagePaths.AccreditationLanding)]
    [FeatureGate(FeatureFlags.ShowAccreditation)]
    public class AccreditationController : Controller
    {
        [Route(template: PagePaths.ApplicationSaved, Name = PagePaths.ApplicationSaved)]
        public IActionResult ApplicationSaved()
        {
            return View();
        }
    }
}
