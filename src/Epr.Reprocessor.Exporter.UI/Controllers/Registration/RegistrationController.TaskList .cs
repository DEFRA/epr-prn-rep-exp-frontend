using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.TaskList)]
    public async Task<IActionResult> TaskList()
    {
        var model = new TaskListModel();
        model.TaskList = CreateViewModel();
        return View(model);
    }
}