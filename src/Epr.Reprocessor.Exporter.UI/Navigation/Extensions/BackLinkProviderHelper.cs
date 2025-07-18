namespace Epr.Reprocessor.Exporter.UI.Navigation.Extensions;

/// <summary>
/// Static helper class for back link-related utilities.
/// </summary>
public static class BackLinkProviderHelper
{
    /// <summary>
    /// Determines the name of the back link handler action for a given controller.
    /// This can be extended to support conventions or annotations if needed.
    /// </summary>
    /// <param name="controller">The controller instance from which the back link is being generated.</param>
    public static string GetActionName(object controller)
    {
        if (controller is IBackLinkAwareController)
        {
            return "on-back-handler";
        }

        throw new InvalidOperationException($"Controller does not implement {nameof(IBackLinkAwareController)}.");
    }
}