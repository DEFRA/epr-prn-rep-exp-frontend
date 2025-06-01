namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Provides a means to add journey tracking to a session. Also works as a marker interface.
/// </summary>
public interface IHasJourneyTracking
{
    /// <summary>
    /// Tracks the journey of pages the user has visited in the registration process.
    /// </summary>
    public List<string> Journey { get; set; }
}