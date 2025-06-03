using System.Text.Json.Serialization;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents a session for the registration application.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegistrationApplicationSession
{
    /// <summary>
	/// Details of the reprocessing site.
	/// </summary>
	public ReprocessingSite? ReprocessingSite { get; set; } = new();

    /// <summary>
    /// Collection of tasks required to be completed as part of the reprocessor registration journey.
    /// </summary>
    [JsonInclude]
	public IList<TaskItem> Tasks { get; private set; } = new List<TaskItem>
    {
        new() { TaskName = TaskType.SiteAndContactDetails, Url = PagePaths.AddressOfReprocessingSite, Status = TaskStatus.NotStart },
        new() { TaskName = TaskType.WasteLicensesPermitsExemptions, Url = "#", Status = TaskStatus.CannotStartYet },
        new() { TaskName = TaskType.ReprocessingInputsOutputs, Url = "#", Status = TaskStatus.CannotStartYet },
        new() { TaskName = TaskType.SamplingAndInspectionPlan, Url = "#", Status = TaskStatus.CannotStartYet },
    };

    /// <summary>
    /// Sets the specified task to 'Completed'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationApplicationSession SetTaskAsComplete(TaskType taskName)
    {
        Tasks.Single(o => o.TaskName == taskName).SetCompleted();

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'InProgress'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationApplicationSession SetTaskAsInProgress(TaskType taskName)
    {
        Tasks.Single(o => o.TaskName == taskName).SetInProgress();

        return this;
    }

    /// <summary>
    /// Sets the specified task to 'NotStarted'.
    /// </summary>
    /// <param name="taskName">The name of the task.</param>
    /// <returns>This instance.</returns>
    public RegistrationApplicationSession SetTaskAsNotStarted(TaskType taskName)
    {
        Tasks.Single(o => o.TaskName == taskName).SetNotStarted();

        return this;
    }
}