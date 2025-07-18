namespace Epr.Reprocessor.Exporter.UI.Navigation.Filters;

/// <summary>
/// Provides options for journey tracking on a session. Can be applied to an action method to set specific options for that method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class JourneyTrackingOptionsAttribute : Attribute
{
    /// <summary>
    /// Sets whether to exclude the action method from journey tracking.
    /// </summary>
    public bool ExcludeFromTracking { get; set; } = false;

    /// <summary>
    /// Sets the page path to be used for the action method in journey tracking instead of the one that is automatically used from the Request.Path
    /// </summary>
    public string? PagePath { get; set; }

    /// <summary>
    /// Whether to allow duplicate entries for this action method and associated page.
    /// </summary>
    public bool AllowDuplicateEntries { get; set; } = false;
}