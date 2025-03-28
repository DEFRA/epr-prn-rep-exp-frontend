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
    [Route(template: PagePath.SelectAuthority, Name = PagePath.SelectAuthority)]
    public IActionResult SelectAuthority()
    {
        return View(new SelectAuthorityModel());
    }

    [IgnoreAntiforgeryToken]
    //[ValidateAntiForgeryToken]
    [HttpPost]
    [Route(template: PagePath.SelectAuthority, Name = PagePath.SelectAuthority)]
    public IActionResult SelectAuthority(
        SelectAuthorityModel model,
        string action)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        //var model = new SelectAuthorityModel();
        if (action == "continue")
        {
            // Handle "Save and continue" logic here
            // For example, redirect to the next page
            return RedirectToRoute(routeName: PagePath.CheckAnswers);// consider using redirect to action for redicts to same controller
            //return RedirectToAction(nameof(AccreditationController.PrnTonnage)); 
        }
        else if (action == "save")
        {
            // Handle "Save and come back later" logic here
         

            return RedirectToRoute(routeName: PagePath.ApplicationSaved);
        }

        return View(model);
    }
}
