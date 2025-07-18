using Epr.Reprocessor.Exporter.UI.Controllers.ControllerExtensions;
using Epr.Reprocessor.Exporter.UI.Navigation.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

/// <summary>
/// A dynamic back link resolver that uses a session-based journey history to
/// determine the appropriate "back" URL.
/// 
/// This is ideal for multi-step forms or wizards where you maintain a stack
/// of previously visited pages and want to return to the step before the current one.
/// 
/// If no prior step exists in the session, a fallback is returned.
/// </summary>
/// <param name="session">Session manager for retrieving journey state.</param>
/// <param name="httpContextAccessor">Used to access the current HTTP context and session.</param>
/// <param name="linkGenerator">Generates a URL back to the controller’s back link handler.</param>
public class JourneyBackLinkResolver(
    ISessionManager<ReprocessorRegistrationSession> session,
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator)
    : IBackLinkResolver
{
    public async Task<string> ResolveAsync(ActionExecutingContext context)
    {
        var sessionData = await session.GetSessionAsync(httpContextAccessor.HttpContext!.Session);
        var journey = sessionData!.Journey;

        // If there's no history, return empty to avoid creating a broken back link.
        if (journey.Count is 0)
        {
            return string.Empty;
        }

        // Get the previous URI in the journey that isn't the current one.
        var redirectUri = journey!.PreviousOrDefault(match: o => o.EndsWith(httpContextAccessor.HttpContext.Request.Path)) ?? string.Empty;

        // Generate a URL that points to the controller’s back link handler
        // passing along the previous path as a redirect parameter.
        var backLink = linkGenerator.GetPathByAction(httpContextAccessor.HttpContext!,
            BackLinkProviderHelper.GetActionName(context.Controller),
            context.RouteData.Values["controller"]!.ToString()!.RemoveControllerFromName(),
            new { redirectTo = redirectUri });

        return backLink ?? string.Empty;
    }
}