namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Defines a dto to carry the details of a registration task, this is not specific to a material but instead is specific to the overall registration.
/// It is different to the regulator task status but can be updated by them and not all statuses are applicable to all tasks.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicantRegistrationTaskDto
{
    /// <summary>
    /// The unique identifier for the task.
    /// </summary>
    public required Guid? Id { get; set; }

    /// <summary>
    /// The name of the task.
    /// </summary>
    public required string TaskName { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Whether the task is a material specific one.
    /// </summary>
    [JsonIgnore]
    public bool IsMaterialSpecific => TaskName is
        nameof(TaskType.WasteLicensesPermitsAndExemptions) or
        nameof(TaskType.ReprocessingInputsAndOutputs) or
        nameof(TaskType.SamplingAndInspectionPlan);
}