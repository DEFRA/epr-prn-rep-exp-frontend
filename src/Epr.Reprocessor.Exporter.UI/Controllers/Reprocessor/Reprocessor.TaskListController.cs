using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using EPR.Common.Authorization.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Reprocessor;

//[Authorize(Policy = PolicyConstants.RegulatorBasicPolicy)]
public class Reprocessor : Controller
{
    [HttpGet]
    [Route(PagePaths.TaskList)]
    public async Task<IActionResult> TaskList()
    {
        var model = new TaskListModel();
        model.TaskList = CreateViewModel();
        return View(model);
    }

    private List<TaskItem> CreateViewModel()
    {
        var lst = new List<TaskItem>();

        // TODO: add logic from data model.

        // if section has data then in progress unless it is completed.
        lst.Add(new TaskItem { TaskName= "Site address and contact details", TaskLink="#",status=TaskListStatus.NotStart });
        // if site section completed can be started, if section has data the in progress unless completed
        lst.Add(new TaskItem { TaskName = "Waste licenses, permits and exemptions", TaskLink = "#", status = TaskListStatus.CannotStartYet });
        // if site section and licenses completed can be started, if section has data the in progress unless completed
        lst.Add(new TaskItem { TaskName = "Reprocessing inputs and outputs", TaskLink = "#", status = TaskListStatus.InProgress });
        // if site section and licenses and Reprocessing completed can be started, if section has data the in progress unless completed
        lst.Add(new TaskItem { TaskName = "Sampling and inspection plan per material", TaskLink = "#", status = TaskListStatus.COMPLETED });

        return lst;
    }
}
