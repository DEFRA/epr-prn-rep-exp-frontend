using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class AccreditationController : Controller
{
    [HttpGet]
    public async Task<IActionResult> PrnTonnage()
    {
        var viewModel = new PrnTonnageViewModel()
        {
            MaterialName = "steel"
        };

        return View(viewModel);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> PrnTonnage(PrnTonnageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        return RedirectToAction("Index");

    }


    [HttpGet]
    [Route(PagePath.SelectAuthority)]
    public IActionResult SelectAuthority()
    {
        return View(new SelectAuthorityModel());
    }
}
