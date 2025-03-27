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
}
