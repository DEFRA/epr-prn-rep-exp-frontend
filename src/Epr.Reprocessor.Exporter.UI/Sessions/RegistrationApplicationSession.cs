namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents a session for the registration application.
/// </summary>
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
	/// Represents packaging waste you will reprocess.
	/// </summary>
	public ReprocessingInputsAndOutputs ReprocessingInputsAndOutputs { get; set; } = new();

	/// <summary>
	/// Contains the registration tasks associated with reprocessor registration journey.
	/// </summary>
	public RegistrationTasks RegistrationTasks { get; set; } = new();
}