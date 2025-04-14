using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.Middleware;

public class AnalyticsCookieMiddleware
{
    private readonly RequestDelegate _next;

    public AnalyticsCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ICookieService cookieService,
        IOptions<GoogleAnalyticsOptions> googleAnalyticsOptions)
    {
        httpContext.Items[ContextKeys.UseGoogleAnalyticsCookieKey] = cookieService.HasUserAcceptedCookies(httpContext.Request.Cookies);
        httpContext.Items[ContextKeys.TagManagerContainerIdKey] = googleAnalyticsOptions.Value.TagManagerContainerId;

        await _next(httpContext);
    }
}