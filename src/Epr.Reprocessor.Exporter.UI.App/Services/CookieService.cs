using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class CookieService : ICookieService
{
    private readonly ILogger<CookieService> _logger;
    private readonly CookieOptions _cookieOptions;
    private readonly GoogleAnalyticsOptions _googleAnalyticsOptions;

    public CookieService(
        ILogger<CookieService> logger,
        IOptions<CookieOptions> eprCookieOptions,
        IOptions<GoogleAnalyticsOptions> googleAnalyticsOptions)
    {
        _logger = logger;
        _cookieOptions = eprCookieOptions.Value;
        _googleAnalyticsOptions = googleAnalyticsOptions.Value;
    }

    public void SetCookieAcceptance(bool accept, IRequestCookieCollection cookies, IResponseCookies responseCookies)
    {
        try
        {
            if (!accept)
            {
                var existingCookies = cookies?.Where(c => c.Key.StartsWith(_googleAnalyticsOptions.CookiePrefix)).ToList();

                if (existingCookies != null)
                {
                    foreach (var cookie in existingCookies)
                    {
                        responseCookies.Append(
                            key: cookie.Key,
                            value: cookie.Value,
                            options: new Microsoft.AspNetCore.Http.CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddYears(-1),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                            });
                    }
                }
            }

            var cookieName = _cookieOptions.CookiePolicyCookieName;
            ArgumentNullException.ThrowIfNull(cookieName);

            responseCookies.Append(
                key: cookieName,
                value: accept.ToString(),
                options: new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMonths(_cookieOptions.CookiePolicyDurationInMonths),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cookie acceptance to '{S}'", accept.ToString());
            throw;
        }
    }

    public bool HasUserAcceptedCookies(IRequestCookieCollection cookies)
    {
        bool cookieAcceptedResult;
        try
        {
            var cookieName = _cookieOptions.CookiePolicyCookieName;
            ArgumentNullException.ThrowIfNull(cookieName);

            var cookie = cookies[cookieName];
            cookieAcceptedResult = bool.TryParse(cookie, out bool cookieAccepted) && cookieAccepted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading cookie acceptance");
            throw;
        }

        return cookieAcceptedResult;
    }
}