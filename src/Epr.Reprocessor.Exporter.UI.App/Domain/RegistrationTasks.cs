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
    public List<TaskItem> Items { get; set; } = [];

    public RegistrationTasks Initialise()
    {
        Items =
        [
            new()
            {
                TaskName = TaskType.SiteAddressAndContactDetails, Url = PagePaths.AddressOfReprocessingSite,
                Status = TaskStatus.NotStart
            },
            new()
            {
                TaskName = TaskType.WasteLicensesPermitsAndExemptions, Url = PagePaths.WastePermitExemptions,
                Status = TaskStatus.CannotStartYet
            },
            new()
            {
                TaskName = TaskType.ReprocessingInputsAndOutputs, Url = PagePaths.ReprocessingInputOutput,
                Status = TaskStatus.CannotStartYet
            },
            new()
            {
                TaskName = TaskType.SamplingAndInspectionPlan, Url = PagePaths.RegistrationSamplingAndInspectionPlan,
                Status = TaskStatus.CannotStartYet
            }
        ];

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'Completed'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsComplete(TaskType taskName)
    {
        Items.Single(o => o.TaskName == taskName).Completed();

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'InProgress'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsInProgress(TaskType taskName)
    {
        Items.Single(o => o.TaskName == taskName).Started();

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'NotStarted'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationTasks SetTaskAsNotStarted(TaskType taskName)
    {
        Items.Single(o => o.TaskName == taskName).NotStarted();

        return this;
    }
}