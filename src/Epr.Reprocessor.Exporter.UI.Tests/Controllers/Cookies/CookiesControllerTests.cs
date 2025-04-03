using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.Cookies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers.Cookies;

[TestClass]
public class CookiesControllerTests
{
    ControllerContext _controllerContext;
    Mock<ICookieService> _cookieServiceMock;
    IOptions<CookieOptions> _CookieOptions;

    [TestInitialize]
    public void TestInitialize()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Cookies = Mock.Of<IRequestCookieCollection>();

        _controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _cookieServiceMock = new Mock<ICookieService>();

        _CookieOptions = Options.Create(new CookieOptions
        {
            SessionCookieName = "sessionCookie",
            B2CCookieName = "b2cCookie",
            CookiePolicyCookieName = "policyCookie",
            AntiForgeryCookieName = "antiForgeryCookie",
            AuthenticationCookieName = "authCookie",
            TsCookieName = "tsCookie",
            TempDataCookie = "tempDataCookie"
        });
    }

    [TestMethod]
    [DataRow(CookieAcceptance.Accept, true)]
    [DataRow(CookieAcceptance.Reject, false)]
    public void ShouldUpdateAcceptance(string cookies, bool acceptance)
    {
        // Arrange
        var cookieService = new Mock<ICookieService>();

        using var cookieController = new CookiesController(cookieService.Object, Mock.Of<IOptions<CookieOptions>>(), Mock.Of<IOptions<GoogleAnalyticsOptions>>())
        {
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),
            ControllerContext = _controllerContext
        };

        // Act
        cookieController.UpdateAcceptance("returnUrl", cookies);

        // Assert
        cookieService.Verify(cs => cs.SetCookieAcceptance(acceptance,
            _controllerContext.HttpContext.Request.Cookies,
            _controllerContext.HttpContext.Response.Cookies));
    }

    [TestMethod]
    public void ShouldRedirectToReturnUrlWhenUpdateAcceptance()
    {
        // Arrange
        const string returnUrl = "returnUrl";
        var cookieService = new Mock<ICookieService>();

        using var cookieController = new CookiesController(cookieService.Object, Mock.Of<IOptions<CookieOptions>>(), Mock.Of<IOptions<GoogleAnalyticsOptions>>())
        {
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),

            ControllerContext = _controllerContext
        };

        // Act
        var result = cookieController.UpdateAcceptance(returnUrl, "cookies");

        // Assert
        result.Url.Should().Be(returnUrl);
    }

    [TestMethod]

    public void ShouldRedirectToHomeWhenUrlNotLocal()
    {
        // Arrange
        const string returnUrl = "returnUrl";

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(h => h.IsLocalUrl(returnUrl)).Returns(true);

        using var cookieController = new CookiesController(Mock.Of<ICookieService>(), Mock.Of<IOptions<CookieOptions>>(), Mock.Of<IOptions<GoogleAnalyticsOptions>>())
        {
            Url = urlHelper.Object,
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),
            ControllerContext = _controllerContext
        };

        // Act
        var result = cookieController.AcknowledgeAcceptance(returnUrl);

        // Assert
        result.Url.Should().Be(returnUrl);
    }

    [TestMethod]
    public void AcknowledgeAcceptance_LocalUrl_RedirectsToThatUrl()
    {
        // Arrange
        string localUrl = "/local/path";
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.IsLocalUrl(localUrl)).Returns(true);
        using var cookieController = new CookiesController(_cookieServiceMock.Object, _CookieOptions, Mock.Of<IOptions<GoogleAnalyticsOptions>>())
        {
            Url = urlHelper.Object,
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),
            ControllerContext = _controllerContext
        };

        // Act
        var result = cookieController.AcknowledgeAcceptance(localUrl);

        // Assert
        Assert.IsInstanceOfType(result, typeof(LocalRedirectResult));
        var redirectResult = (LocalRedirectResult)result;
        Assert.AreEqual(localUrl, redirectResult.Url);
    }

    private object ViewBagValue(ViewResult viewResult, string key)
    {
        return viewResult.ViewData[key];
    }
}
