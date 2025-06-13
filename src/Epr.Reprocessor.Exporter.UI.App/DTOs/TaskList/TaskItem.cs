namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Represents a model for an individual task entry.
/// </summary>
[ExcludeFromCodeCoverage]
public class TaskItem
{
    public required Guid Id { get; set; }
    /// <summary>
    /// The name of the task, this can power the display by using the Display attribute to set the display text.
    /// </summary>
    public string TaskName { get; set; }
    public TaskType TaskType =>
        TaskName switch
        {
            "Site address and contact details" => TaskType.SiteAndContactDetails,
            "Waste licenses, permits and exemptions" => TaskType.WasteLicensesPermitsExemptions,
            "Reprocessing inputs and outputs" => TaskType.ReprocessingInputsOutputs,
            "Sampling and inspection plan per material" => TaskType.SamplingAndInspectionPlan,
            _ => TaskType.Unknown
        };

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
	public Enums.TaskStatus TaskStatus { get
	{
	    return Status switch
	    {
	        "CANNOT START YET" => Enums.TaskStatus.CannotStartYet,
	        "NOT STARTED" => Enums.TaskStatus.NotStart,
	        "IN PROGRESS" => Enums.TaskStatus.InProgress,
	        "COMPLETED" => Enums.TaskStatus.Completed,
	        _ => throw new InvalidOperationException($"Unknown status: {Status}")
	    };
	} }
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

        return this;
    }

    /// <summary>
    /// Sets the task status to 'InProgress'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetInProgress()
    {
        Status = "IN PROGRESS";

        return this;
    }

    /// <summary>
    /// Sets the task status to 'NotStarted'.
    /// </summary>
    /// <returns>This instance.</returns>
    public TaskItem SetNotStarted()
    {
        Status = "NOT STARTED";

        return this;
    }
}