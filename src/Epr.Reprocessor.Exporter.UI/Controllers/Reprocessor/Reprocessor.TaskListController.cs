using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Reprocessor;

//[Authorize(Policy = PolicyConstants.RegulatorBasicPolicy)]
public class Reprocessor : Controller
{
    private readonly IStringLocalizer<Reprocessor> _localizer;

    public Reprocessor(IStringLocalizer<Reprocessor> localizer)
    {
        _localizer = localizer;
    }

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
        var sessionData = new ReprocessorSessionData();

        // TODO: add logic from data model.
        lst = CalculateTaskListStatus(sessionData);
  
        return lst;
    }

    private List<TaskItem> CalculateTaskListStatus(ReprocessorSessionData sessionData)
    {
        var lst = new List<TaskItem>();
        // if new then use defualt values
        if (true)
        { 
            lst.Add(new TaskItem { TaskName = _localizer["RxExRegistrationTaskList.SiteAddressAndContactDetails"], TaskLink = "#", status = TaskListStatus.NotStart }); 
            lst.Add(new TaskItem { TaskName = _localizer["RxExRegistrationTaskList.WasteLicensesPermitsAndExemptions"], TaskLink = "#", status = TaskListStatus.CannotStartYet }); 
            lst.Add(new TaskItem { TaskName = _localizer["RxExRegistrationTaskList.ReprocessingInputsAndOutputs"], TaskLink = "#", status = TaskListStatus.CannotStartYet }); 
            lst.Add(new TaskItem { TaskName = _localizer["RxExRegistrationTaskList.SamplingAndInspectionPlanPerMateria"], TaskLink = "#", status = TaskListStatus.CannotStartYet });
            return lst;
        }
         
        lst.Add(new TaskItem { TaskName = "Site address and contact details", TaskLink = "#", status = sessionData.AddressStatus });
             

        return lst; 
    }
}

internal class ReprocessorSessionData
{
    public ReprocessorSessionData()
    {  
        
    }
    public TaskListStatus AddressStatus = TaskListStatus.COMPLETED;
}