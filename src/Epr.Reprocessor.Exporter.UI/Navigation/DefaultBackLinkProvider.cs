using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.Navigation;

/// <summary>
/// Default implementation of IBackLinkProvider that checks if the controller wants to override
/// back link behavior. If not, it falls back to a system-wide default resolver.
/// </summary>
/// <param name="defaultResolver">The default resolver used when a controller does not supply its own.</param>
public class DefaultBackLinkProvider(IBackLinkResolver defaultResolver) : IBackLinkProvider
{
    private readonly IBackLinkResolver _defaultResolver = defaultResolver;

    public async Task<string> GetBackLinkAsync(ActionExecutingContext context)
    {
        if (context.Controller is IBackLinkAwareController backLinkAware &&
            backLinkAware.TryGetBackLinkResolver(out var resolver) &&
            resolver != null)
        {
            // Use controller-specific resolver if available
            return await resolver.ResolveAsync(context);
        }

        // Fallback to default resolver
        return await _defaultResolver.ResolveAsync(context);
    }
}