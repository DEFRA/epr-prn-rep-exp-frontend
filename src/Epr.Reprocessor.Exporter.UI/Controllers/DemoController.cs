using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [AllowAnonymous]
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
			return View();
		}
    }
}
