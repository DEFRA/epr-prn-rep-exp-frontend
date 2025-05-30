namespace Epr.Reprocessor.Exporter.UI.Attributes;

/// <summary>
/// Defines an attribute that is used as a means to specify overriding options for the journey tracker.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[ExcludeFromCodeCoverage]
public class JourneyTrackingOptionsAttribute : Attribute
{
    /// <summary>
    /// Whether to exclude the action method that this is applied to from being tracked in the user's journey.
    /// </summary>
    public bool ExcludeFromTracking { get; set; } = false;

    /// <summary>
    /// Custom page to use as the path for the journey tracker.
    /// </summary>
    public string? PagePath { get; set; }
}