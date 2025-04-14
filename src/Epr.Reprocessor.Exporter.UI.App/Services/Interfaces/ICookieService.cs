using Microsoft.AspNetCore.Http;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface ICookieService
{
    void SetCookieAcceptance(bool accept, IRequestCookieCollection cookies, IResponseCookies responseCookies);

    bool HasUserAcceptedCookies(IRequestCookieCollection cookies);
}