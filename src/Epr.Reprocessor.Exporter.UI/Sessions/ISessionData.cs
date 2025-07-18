namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Acts as a marker interface to indicate that this is a session data class, used as a constraint on the generically typed journey tracker attribute.
/// Without this on your parent session object, you will not be able to use the journey tracker attribute.
/// </summary>
public interface ISessionData : IHasJourneyTracking
{
}