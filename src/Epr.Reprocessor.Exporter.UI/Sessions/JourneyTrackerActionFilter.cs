using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Defines an action filter that ensures that the journey the user undertakes is tracked in session.
/// </summary>
/// <typeparam name="T">Generic type parameter.</typeparam>
/// <param name="sessionManager">Provides access to the <see cref="HttpContext.Session"/>.</param>
[AttributeUsage(AttributeTargets.Class)]
[ExcludeFromCodeCoverage]
public class JourneyTrackerActionFilter<T>(ISessionManager<T> sessionManager) 
    : ActionFilterAttribute where T : class, IHasJourneyTracking, new()
{
    /// <inheritdoc/>>.
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = await sessionManager.GetSessionAsync(context.HttpContext.Session);

        if (session is null)
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        // Get the relative URL of the current request
        var url = GetRelativeUrl(context.HttpContext.Request.Path);

        // If the url doesn't already exist in the journey, add it.
        if (!session.Journey.Contains(url))
        {
            var endpoint = context.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<JourneyTrackingOptionsAttribute>();

            if (attribute is not null)
            {
                if (attribute.ExcludeFromTracking)
                {
                    // Skip tracking for this action
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }

                if (!string.IsNullOrEmpty(attribute.PagePath))
                {
                    session.Journey.Add(url);
                }
            }
            else
            {
                session.Journey.Add(url);
            }
        }

        await sessionManager.SaveSessionAsync(context.HttpContext.Session, session);
        
        await base.OnActionExecutionAsync(context, next);
    }

    private static string GetRelativeUrl(string path)
    {
        string relativePath;

        if (path.StartsWith('/'))
        {
            path = path.Substring(1);
        }

        if (path.StartsWith(PagePaths.RegistrationLanding, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = path.Substring(PagePaths.RegistrationLanding.Length);
            
            // Remove leading slash if present
            if (relativePath.StartsWith('/'))
            {
                relativePath = relativePath.Substring(1);
            }
        }
        else
        {
            relativePath = path;
        }

        return relativePath;
    }
}