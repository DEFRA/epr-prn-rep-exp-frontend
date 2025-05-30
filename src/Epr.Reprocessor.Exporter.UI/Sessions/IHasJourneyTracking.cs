namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Provides a contract for a session object that wants to have a journey tracker.
/// This interface also serves as a marker interface for use in the <see cref="JourneyTrackerActionFilter{T}"/>.
/// </summary>
public interface IHasJourneyTracking
{
    /// <summary>
    /// Tracks the journey of pages the user has visited in the registration process.
    /// </summary>
    public List<string> Journey { get; set; }
}