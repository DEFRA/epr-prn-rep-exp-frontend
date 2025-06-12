using Epr.Reprocessor.Exporter.UI.App.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    [ExcludeFromCodeCoverage]
    [Route(PagePaths.ExporterPlaceholder)]
	public class ExporterPlaceholder : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
