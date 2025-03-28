using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class AccreditationController : Controller
{
    public IActionResult PrnTonnage()
    {
        return View();
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
