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
        Items.Single(o => o.TaskType == taskName).SetCompleted();

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
            Items = new List<TaskItem>();
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
        Items.Single(o => o.TaskType == taskName).SetNotStarted();

        return this;
    }
}