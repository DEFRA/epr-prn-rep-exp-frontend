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

    [IgnoreAntiforgeryToken]
    //[ValidateAntiForgeryToken]
    [HttpPost]
    [Route(PagePath.SelectAuthority)]
    public IActionResult SelectAuthority(
        SelectAuthorityModel model,
        string action)
    {
        //var model = new SelectAuthorityModel();
        if (action == "continue")
        {
            // Handle "Save and continue" logic here
            // For example, redirect to the next page
            return RedirectToAction("NextPage");
        }
        else if (action == "save")
        {
            // Handle "Save and come back later" logic here
            // For example, save the data and stay on the same page
            // You might want to add a message indicating the data was saved
            ViewBag.Message = "Data saved. You can come back later.";
            return View(model);
        }

        return View(model);
    }
}
