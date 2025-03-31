using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.FeatureManagement.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

[FeatureGate(FeatureFlags.ShowAccreditation)]
public class AccreditationController : Controller
{
    [HttpGet]
    [Route(template: PagePath.SelectPrnTonnage, Name = PagePath.SelectPrnTonnage)]
    public async Task<IActionResult> PrnTonnage()
    {
        var viewModel = new PrnTonnageViewModel()
        {
            MaterialName = "steel"
        };

        return View(viewModel);
    }

    [HttpPost]
    [Route(template: PagePath.SelectPrnTonnage, Name = PagePath.SelectPrnTonnage)]
    public async Task<IActionResult> PrnTonnage(PrnTonnageViewModel viewModel, string action)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        // Save logic TBC.

        if (action == "continue")
        {
            return RedirectToRoute(routeName: PagePath.SelectAuthority);
        }
        else if (action == "save")
        {
            return RedirectToRoute(routeName: PagePath.ApplicationSaved);
        }        

        return View(viewModel);
    }

    [HttpGet]
    [Route(template: PagePath.SelectAuthority, Name = PagePath.SelectAuthority)]
    public async Task<IActionResult> SelectAuthority()
    {
        var model = new SelectAuthorityModel();
        model.Authorities.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "paul", Text = "Paul Reprocessor", Group = new SelectListGroup { Name = "paul.Reprocessor@reprocessor.com" } });
        model.Authorities.AddRange([
             new SelectListItem { Value = "myself", Text = "Myself", Group = new SelectListGroup { Name = "Myself@reprocessor.com" } },
                    new SelectListItem { Value = "andrew", Text = "Andrew Recycler", Group = new SelectListGroup { Name = "Andrew.Recycler@reprocessor.com" } },
                    new SelectListItem { Value = "gary1", Text = "Gary Package", Group = new SelectListGroup { Name = "Gary.Package1@reprocessor.com" } },
                    new SelectListItem { Value = "gary2", Text = "Gary Package", Group = new SelectListGroup { Name = "GaryWPackageP@reprocessor.com" } },
                    new SelectListItem { Value = "scott", Text = "Scott Reprocessor", Group = new SelectListGroup { Name = "Scott.Reprocessor@reprocessor.com" } }
                   ] );
        model.SelectedAuthorities.AddRange(["myself", "andrew"]);

        return View(model);
    }

    [IgnoreAntiforgeryToken]
    //[ValidateAntiForgeryToken]
    [HttpPost]
    [Route(template: PagePath.SelectAuthority, Name = PagePath.SelectAuthority)]
    public async Task<IActionResult> SelectAuthority(
        SelectAuthorityModel model,
        string action)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

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

    [Route(template: PagePath.CheckAnswers, Name = PagePath.CheckAnswers)]
    public async Task<IActionResult> CheckAnswers()
    {
        return View();
    }
}
