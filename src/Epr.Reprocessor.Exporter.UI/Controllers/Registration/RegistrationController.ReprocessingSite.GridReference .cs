using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [ExcludeFromCodeCoverage]
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        return View();
    }
}