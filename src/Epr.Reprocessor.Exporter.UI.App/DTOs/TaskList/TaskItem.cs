using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Represents a model for an individual task entry.
/// </summary>
[ExcludeFromCodeCoverage]
public class TaskItem
{
    /// <summary>
    /// The unique identifier for the task, this can be null if the task has not yet been created in the backend.
    /// </summary>
    public Guid? Id { get; set; }

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
    public ApplicantRegistrationTaskStatus Status { get; set; }

    /// <summary>
    /// Flag for future proofing, acts as a feature flag so that can add new tasks but keep them hidden from view if required.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines whether this task is material specific.
    /// </summary>
    public bool IsMaterialSpecific { get; set; }

    /// <summary>
    /// Creates a new task item entry.
    /// </summary>
    /// <param name="id">The unique identifier for the task, can be null if the task has not yet been created in the backend.</param>
    /// <param name="taskName">The name of the task.</param>
    /// <param name="status">The status to set the task.</param>
    /// <returns>This instance.</returns>
    public TaskItem Create(Guid? id, string taskName, string status)
    {
        Id = id;
        TaskName = Enum.Parse<TaskType>(taskName);
        Status = MapStatus(status);
        Url = ReprocessorExporterTaskTypeUrlProvider.Url(TaskName);
        IsMaterialSpecific = TaskName is TaskType.WasteLicensesPermitsAndExemptions or TaskType.ReprocessingInputsAndOutputs;

        return this;
    }

    /// <summary>
    /// Sets the task status to 'Completed'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem Completed()
    {
        Status = ApplicantRegistrationTaskStatus.Completed;

        return this;
    }

    /// <summary>
    /// Sets the task status to 'InProgress'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem Started()
    {
        Status = ApplicantRegistrationTaskStatus.Started;

        return this;
    }

    /// <summary>
    /// Sets the task status to 'NotStarted'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem NotStarted()
    {
        Status = ApplicantRegistrationTaskStatus.NotStarted;

        return this;
    }

    private static ApplicantRegistrationTaskStatus MapStatus(string status) =>
        Enum.Parse<ApplicantRegistrationTaskStatus>(status);
}