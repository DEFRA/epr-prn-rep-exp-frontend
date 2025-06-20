namespace Epr.Reprocessor.Exporter.UI.Middleware;

/// <summary>
/// Middleware to ensure that a session of type <see cref="ReprocessorRegistrationSession"/> is created.
/// </summary>
/// <param name="next">A delegate to the next middleware in the pipeline.</param>
[ExcludeFromCodeCoverage]
public class EnsureSessionCreatedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        ILogger<EnsureSessionCreatedMiddleware> logger)
    {
        // Ensure the session is created
        if (context.Session.IsAvailable && context.User.Identity is not null && context.User.Identity.IsAuthenticated)
        {
            logger.LogInformation($"No session of type {nameof(ReprocessorRegistrationSession)}, creating.");

            var session = await sessionManager.GetSessionAsync(context.Session);

            if (session is null)
            {
                await sessionManager.SaveSessionAsync(context.Session, new ReprocessorRegistrationSession());
            }
        }

        // Call the next middleware in the pipeline
        await next(context);
    }
}