using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.Navigation;

/// <summary>
/// Abstraction for providing back links from anywhere in the application.
/// This is the top-level service that resolves the appropriate back link for the current context.
/// </summary>
public interface IBackLinkProvider
{
    /// <summary>
    /// Resolve the back link using either a controller-defined resolver or the default fallback resolver.
    /// </summary>
    Task<string> GetBackLinkAsync(ActionExecutingContext context);
}