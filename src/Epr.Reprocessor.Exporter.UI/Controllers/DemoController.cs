using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.Controllers
{
    [AllowAnonymous]
    public class DemoController : Controller
    {
        private readonly LinkSettings _linkSettings;

        public DemoController(IOptions<LinkSettings> linkSettings)
        {
            _linkSettings = linkSettings.Value;
        }

        public IActionResult Index()
        {
            var viewModel = new LinkSettings
            {
                AddOrganisation = _linkSettings.AddOrganisation,
                ViewOrganisations = _linkSettings.ViewOrganisations,
                ApplyReprocessor = _linkSettings.ApplyReprocessor,
                ApplyExporter = _linkSettings.ApplyExporter,
                ViewApplications = _linkSettings.ViewApplications
            };
            return View(viewModel);
		}
    }
}
