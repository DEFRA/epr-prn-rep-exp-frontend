using EPR.Common.Authorization.Sessions;
using Epr.Reprocessor.Exporter.UI.Sessions;

namespace Epr.Reprocessor.Exporter.UI.Middleware;

public class EnsureSessionCreatedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        ILogger<EnsureSessionCreatedMiddleware> logger)
    {
        // Ensure the session is created
        if (!context.Session.IsAvailable)
        {
            if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
            {
                logger.LogInformation($"No session of type {nameof(ReprocessorRegistrationSession)}, creating.");

                var session = await sessionManager.GetSessionAsync(context.Session);

                if (session is null)
                {
                    await sessionManager.SaveSessionAsync(context.Session, new ReprocessorRegistrationSession());
                }
            }
        }

        // Call the next middleware in the pipeline
        await next(context);
    }
}