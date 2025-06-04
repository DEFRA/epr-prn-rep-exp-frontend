using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Represents a model for an individual task entry.
/// </summary>
[ExcludeFromCodeCoverage]
public class TaskItem
{
    /// <summary>
    /// The name of the task, this can power the display by using the Display attribute to set the display text.
    /// </summary>
    public TaskType TaskName { get; set; }

    /// <summary>
    /// The url that the task links to, can be null if the task entry isn't activated as a link due to business logic.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
	public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Flag for future proofing, acts as a feature flag so that can add new tasks but keep them hidden from view if required.
    /// </summary>
	public bool Enabled { get; set; } = true;

    /// <summary>
    /// Sets the task status to 'Completed'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetCompleted()
    {
        Status = Enums.TaskStatus.Completed;

        return this;
    }

    /// <summary>
    /// Sets the task status to 'InProgress'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetInProgress()
    {
        Status = Enums.TaskStatus.InProgress;

        return this;
    }

    /// <summary>
    /// Sets the task status to 'NotStarted'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetNotStarted()
    {
        Status = Enums.TaskStatus.NotStart;

        return this;
    }
}