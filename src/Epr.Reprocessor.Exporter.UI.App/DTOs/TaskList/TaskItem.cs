using Newtonsoft.Json.Linq;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Represents a model for an individual task entry.
/// </summary>
[ExcludeFromCodeCoverage]
public class TaskItem
{
    public string Id { get; set; }
    /// <summary>
    /// The name of the task, this can power the display by using the Display attribute to set the display text.
    /// </summary>
    public string TaskName { get; set; }

    private TaskType? _taskType;
    public TaskType TaskType
    {
        get
        {
            if (_taskType == null)
            {
                return
                    TaskName switch
                    {
                        "SiteAndContactDetails" => TaskType.SiteAndContactDetails,
                        "WasteLicensesPermitsExemptions" => TaskType.WasteLicensesPermitsExemptions,
                        "ReprocessingInputsOutputs" => TaskType.ReprocessingInputsOutputs,
                        "SamplingAndInspectionPlan" => TaskType.SamplingAndInspectionPlan,
                        _ => TaskType.Unknown
                    };
            }
            else
            {
                return (TaskType)_taskType;
            }
        }
        set => _taskType = value;
    }

    /// <summary>
    /// The url that the task links to, can be null if the task entry isn't activated as a link due to business logic.
    /// </summary>
    private string? _url;
    public string? Url
    {
        get
        {
            if (string.IsNullOrEmpty(_url))
            {
                return TaskType switch
                {
                    TaskType.SiteAndContactDetails => PagePaths.AddressOfReprocessingSite,
                    TaskType.WasteLicensesPermitsExemptions => PagePaths.WastePermitExemptions,
                    TaskType.ReprocessingInputsOutputs => PagePaths.ReprocessingInputOutput,
                    TaskType.SamplingAndInspectionPlan => PagePaths.RegistrationSamplingAndInspectionPlan,
                    _ => null
                };
            }
            else
            {
                return _url;
            }
        }
        set { _url = value; }
    }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    private Enums.TaskStatus? _taskStatus;
    public Enums.TaskStatus TaskStatus
    {
        get
        {
            if (_taskStatus == null)
            {
                return Status switch
                {
                    "CannotStartYet" => (Enums.TaskStatus)Enums.TaskStatus.CannotStartYet,
                    "NotStarted" => (Enums.TaskStatus)Enums.TaskStatus.NotStart,
                    "InProgress" => (Enums.TaskStatus)Enums.TaskStatus.InProgress,
                    "Completed" => (Enums.TaskStatus)Enums.TaskStatus.Completed,
                    _ => throw new InvalidOperationException($"Unknown status: {Status}")
                };
            }
            else
            {
                return (Enums.TaskStatus)_taskStatus;
            }
        }
        set => _taskStatus = value;
    }

    public string Status { get; set; }

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
        Status = "COMPLETED";
        TaskStatus = Enums.TaskStatus.Completed;
        return this;
    }

    /// <summary>
    /// Sets the task status to 'InProgress'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetInProgress()
    {
        Status = "IN PROGRESS";
        TaskStatus = Enums.TaskStatus.InProgress;
        return this;
    }

    /// <summary>
    /// Sets the task status to 'NotStarted'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetNotStarted()
    {
        Status = "NOT STARTED";
        TaskStatus = Enums.TaskStatus.NotStart;
        return this;
    }
}