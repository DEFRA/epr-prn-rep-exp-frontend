using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

namespace Epr.Reprocessor.Exporter.UI.Navigation;

/// <summary>
/// Interface for controllers that want to explicitly participate in back link resolution.
/// </summary>
public interface IBackLinkAwareController
{
    /// <summary>
    /// Attempt to provide a custom back link resolver for the current controller instance.
    /// </summary>
    bool TryGetBackLinkResolver(out IBackLinkResolver? resolver);

    /// <summary>
    /// Optional handler invoked when a back link is followed. Default is a no-op.
    /// Controllers can override this to restore state or redirect.
    /// </summary>
    Task<IActionResult> OnBackHandlerAsync(string redirectTo) => Task.FromResult<IActionResult>(new EmptyResult());
}