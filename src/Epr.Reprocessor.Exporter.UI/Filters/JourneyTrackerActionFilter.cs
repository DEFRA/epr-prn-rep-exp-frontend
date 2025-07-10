using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.Filters;

/// <summary>
/// Defines a filter that tracks the journey of pages visited by the user during the registration process.
/// Can be applied to controller classes or as part of a page model convention to apply to all controllers that match the convention.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// [ServiceFilter(typeof(JourneyTrackerActionFilter&lt;ReprocessorRegistrationSession&gt;))]
/// public class RegistrationController()
/// {
/// }
/// </code>
/// Also works on base classes as the attribute is inherited, i.e.
/// <code>
/// [ServiceFilter(typeof(JourneyTrackerActionFilter&lt;ReprocessorRegistrationSession&gt;))]
/// public class BaseController()
/// {
/// }
/// </code>
/// </remarks>
/// <typeparam name="TSession">The generic typed session parameter to use for this tracker.</typeparam>
/// <param name="sessionManager">Provides an Api to manage session data.</param>
[AttributeUsage(AttributeTargets.Class)]
public class JourneyTrackerActionFilter<TSession>(ISessionManager<TSession> sessionManager) : ActionFilterAttribute
    where TSession : class, ISessionData, new()
{
    /// <inheritdoc />
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await base.OnActionExecutionAsync(context, next);

        var session = await sessionManager.GetSessionAsync(context.HttpContext.Session);
        if (session is null)
        {
            session = new TSession();
            await sessionManager.SaveSessionAsync(context.HttpContext.Session, session);
        }

        if (session.Journey.IndexOf(context.HttpContext.Request.Path) < 0)
        {
            if (((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor)
                .MethodInfo.GetCustomAttribute(typeof(JourneyTrackingOptionsAttribute)) is JourneyTrackingOptionsAttribute attribute)
            {
                if (attribute.ExcludeFromTracking)
                {
                    // Skip tracking for this action
                    await next();
                    return;
                }

                if (!string.IsNullOrEmpty(attribute.PagePath))
                {
                    session.Journey.Add(attribute.PagePath);
                }
            }
            else
            {
                session.Journey.Add(context.HttpContext.Request.GetEncodedPathAndQuery());
            }
        }

        await sessionManager.SaveSessionAsync(context.HttpContext.Session, session);

        await next();
    }
}