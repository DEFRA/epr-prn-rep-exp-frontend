using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [Route(PagePaths.Registration)]
    public class RegistrationController : Controller
    {
        [HttpGet]
        [Route(PagePaths.NoAddressFound)]
        public IActionResult NoAddressFound()
        {
            var postCode = "[TEST POSTCODE REPLACE WITH SESSION]"; // TODO: Get from session

            var model = new NoAddressFoundViewModel { Postcode = postCode };

            return View(model);
        }
    }
}
