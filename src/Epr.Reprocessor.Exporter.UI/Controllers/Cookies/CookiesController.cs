using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;


namespace Epr.Reprocessor.Exporter.UI.Controllers.Cookies;

[AllowAnonymous]
public class CookiesController : Controller
{
    private readonly ICookieService _cookieService;
    private readonly CookieOptions _cookieOptions;
    private readonly GoogleAnalyticsOptions _googleAnalyticsOptions;

    public CookiesController(
        ICookieService cookieService,
        IOptions<CookieOptions> eprCookieOptions,
        IOptions<GoogleAnalyticsOptions> googleAnalyticsOptions)
    {
        _cookieService = cookieService;
        _cookieOptions = eprCookieOptions.Value;
        _googleAnalyticsOptions = googleAnalyticsOptions.Value;
    }

    [Route(PagePaths.Cookies)]
    public IActionResult Detail(string returnUrl, bool? cookiesAccepted = null)
    {
        var allowedBackValues = new List<string> { "/epr-prn", "/create-account", "/manage-account" };
        var validBackLink = !string.IsNullOrWhiteSpace(returnUrl) && allowedBackValues.Exists(a => returnUrl.StartsWith(a));

        string returnUrlAddress = validBackLink ? returnUrl : Url.Content("~/");

        var hasUserAcceptedCookies = cookiesAccepted != null ? cookiesAccepted.Value : _cookieService.HasUserAcceptedCookies(Request.Cookies);

        var cookieViewModel = new CookieDetailViewModel
        {
            SessionCookieName = _cookieOptions.SessionCookieName,
            CookiePolicyCookieName = _cookieOptions.CookiePolicyCookieName,
            AntiForgeryCookieName = _cookieOptions.AntiForgeryCookieName,
            GoogleAnalyticsDefaultCookieName = _googleAnalyticsOptions.DefaultCookieName,
            GoogleAnalyticsAdditionalCookieName = _googleAnalyticsOptions.AdditionalCookieName,
            AuthenticationCookieName = _cookieOptions.AuthenticationCookieName,
            TsCookieName = _cookieOptions.TsCookieName,
            TempDataCookieName = _cookieOptions.TempDataCookie,
            B2CCookieName = _cookieOptions.B2CCookieName,
            CorrelationCookieName = _cookieOptions.CorrelationCookieName,
            OpenIdCookieName = _cookieOptions.OpenIdCookieName,
            CookiesAccepted = hasUserAcceptedCookies,
            ReturnUrl = returnUrlAddress,
            ShowAcknowledgement = cookiesAccepted != null
        };

        ViewBag.BackLinkToDisplay = returnUrlAddress;
        ViewBag.CurrentPage = returnUrlAddress;

        return View(cookieViewModel);
    }

    [HttpPost]
    [Route(PagePaths.Cookies)]
    public IActionResult Detail(string returnUrl, string cookiesAccepted)
    {
        _cookieService.SetCookieAcceptance(cookiesAccepted == CookieAcceptance.Accept, Request.Cookies, Response.Cookies);
        TempData[CookieAcceptance.CookieAcknowledgement] = cookiesAccepted;

        return Detail(returnUrl, cookiesAccepted == CookieAcceptance.Accept);
    }

    [HttpPost]
    [Route(PagePaths.UpdateCookieAcceptance)]
    public LocalRedirectResult UpdateAcceptance(string returnUrl, string cookies)
    {
        _cookieService.SetCookieAcceptance(cookies == CookieAcceptance.Accept, Request.Cookies, Response.Cookies);
        TempData[CookieAcceptance.CookieAcknowledgement] = cookies;

        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    [Route(PagePaths.AcknowledgeCookieAcceptance)]
    public LocalRedirectResult AcknowledgeAcceptance(string returnUrl)
    {
        return LocalRedirect(returnUrl);
    }
}