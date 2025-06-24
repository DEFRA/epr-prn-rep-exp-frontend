namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents details of the current session that is holding details of the registration application.
/// </summary>
[ExcludeFromCodeCoverage]
public class ExporterRegistrationSession : IHasUserData, IHasJourneyTracking
{
    public ExporterRegistrationSession()
    {
    }

    /// <summary>
    /// Data related to the user that is registering on behalf of their org.
    /// </summary>
    public UserData UserData { get; set; } = new();

    /// <summary>
    /// Tracks the journey of pages the user has visited in the registration process.
    /// </summary>
    public List<string> Journey { get; set; } = new();

    /// <summary>
    /// The unique identifier for the registration application.
    /// </summary>
    public Guid? RegistrationId { get; set; }

    /// <summary>
    /// Represents details of the registration application.
    /// </summary>
    public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new();

    //TODO: Check this session in RPD and confirm if we can base our session on it
    //public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new ();
}