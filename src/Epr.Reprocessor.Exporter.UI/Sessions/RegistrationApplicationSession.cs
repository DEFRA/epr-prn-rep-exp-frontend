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
    /// Details of the waste that is to be recycled.
    /// </summary>
    public PackagingWaste? WasteDetails { get; set; } = new();

    /// <summary>
    /// Contains the registration tasks associated with reprocessor registration journey.
    /// </summary>
    public RegistrationTasks RegistrationTasks { get; set; } = new(new List<TaskItem>
    {
        new() { TaskName = TaskType.SiteAndContactDetails, Url = PagePaths.AddressOfReprocessingSite, Status = TaskStatus.NotStart },
        new() { TaskName = TaskType.WasteLicensesPermitsExemptions, Url = "#", Status = TaskStatus.CannotStartYet },
        new() { TaskName = TaskType.ReprocessingInputsOutputs, Url = "#", Status = TaskStatus.CannotStartYet },
        new() { TaskName = TaskType.SamplingAndInspectionPlan, Url = "#", Status = TaskStatus.CannotStartYet },
    });
}