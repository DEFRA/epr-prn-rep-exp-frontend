using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Defines a collection of tasks that are required to be completed as part of the reprocessor registration journey.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegistrationTasks
{
    /// <summary>
    /// List of tasks.
    /// </summary>
    public List<TaskItem> Items { get; set; }  

    /// <summary>
    /// Sets the specified task to 'Completed'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsComplete(TaskType taskName)
    {

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'InProgress'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsInProgress(TaskType taskName)
    {
        if (Items == null)
        {
            CreateDefaultTaskList();
        }
        Items.Single(o => o.TaskType == taskName).SetInProgress();

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'NotStarted'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsNotStarted(TaskType taskName)
    {
        if (Items == null)
        {
            CreateDefaultTaskList();
        }
        Items.Single(o => o.TaskType == taskName).SetNotStarted();

        return this;
    }

    public void CreateDefaultTaskList()
    {
        Items = new List<TaskItem>
        {
            new()
            {
                TaskType = TaskType.SiteAndContactDetails,
                TaskName = "Site address and contact details", Url = PagePaths.AddressOfReprocessingSite,
                Status = "NOT STARTED", Id = Guid.NewGuid().ToString()
            },
            new()
            {
                TaskType = TaskType.WasteLicensesPermitsExemptions,
                TaskName = "Waste licenses, permits and exemptions", Url = PagePaths.WastePermitExemptions,
                Status = "CANNOT START YET", Id = Guid.NewGuid().ToString()
            },
            new()
            {
                TaskType = TaskType.ReprocessingInputsOutputs,
                TaskName = "Reprocessing inputs and outputs", Url = PagePaths.ReprocessingInputOutput,
                Status = "CANNOT START YET", Id = Guid.NewGuid().ToString()
            },
            new()
            {
                TaskType = TaskType.SamplingAndInspectionPlan,
                TaskName = "Sampling and inspection plan per material",
                Url = PagePaths.RegistrationSamplingAndInspectionPlan, Status = "CANNOT START YET", Id = Guid.NewGuid().ToString()
            },
        };
    }
}