using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

/// <summary>
/// Defines a contract for resolving a back link (i.e., a URL that takes the user
/// back to a logical previous step in a journey or flow).
/// 
/// This abstraction allows controllers or services to determine "where the back
/// button should go" dynamically based on the current request context, user state,
/// session, or application-specific logic.
/// </summary>
public interface IBackLinkResolver
{
    /// <summary>
    /// Resolves a back link URL asynchronously given the action execution context.
    /// </summary>
    Task<string> ResolveAsync(ActionExecutingContext context);
}