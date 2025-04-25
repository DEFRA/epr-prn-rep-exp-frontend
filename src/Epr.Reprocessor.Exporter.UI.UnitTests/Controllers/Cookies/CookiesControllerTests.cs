using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.Cookies;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Cookies;
 
[TestClass]
public class CookiesControllerTests
{
    ControllerContext _controllerContext;
    Mock<ICookieService> _cookieServiceMock;
    IOptions<CookieOptions> _cookieOptions;
    IOptions<GoogleAnalyticsOptions> _googleAnalyticsOptions;

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

        _cookieOptions = Options.Create(new CookieOptions
        {
            SessionCookieName = "sessionCookie",
            B2CCookieName = "b2cCookie",
            CookiePolicyCookieName = "policyCookie",
            AntiForgeryCookieName = "antiForgeryCookie",
            AuthenticationCookieName = "authCookie",
            TsCookieName = "tsCookie",
            TempDataCookie = "tempDataCookie"
        });

        _googleAnalyticsOptions = Options.Create(new GoogleAnalyticsOptions
        {
            CookiePrefix =  "pfx",
            MeasurementId = "measurementId", 
        });
    }

    [TestMethod]
    public void Detail_Get_ReturnsViewResultWithCorrectModel()
    {
        // Arrange
        var returnUrl = "/epr-prn";
        var cookiesAccepted = true;
        _cookieServiceMock.Setup(cs => cs.HasUserAcceptedCookies(It.IsAny<IRequestCookieCollection>())).Returns(cookiesAccepted);

        using var controller = new CookiesController(_cookieServiceMock.Object, _cookieOptions, _googleAnalyticsOptions)
        {
            ControllerContext = _controllerContext
        };

        // Act
        var result = controller.Detail(returnUrl, cookiesAccepted) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<CookieDetailViewModel>();
        var model = result.Model as CookieDetailViewModel;
        model.Should().NotBeNull();
        model.CookiesAccepted.Should().Be(cookiesAccepted);
        model.ReturnUrl.Should().Be(returnUrl);
    }

    [TestMethod]
    public void Detail_Post_SetsCookieAcceptanceAndReturnsViewResult()
    {
        // Arrange
        var returnUrl = "/epr-prn";
        var cookiesAccepted = CookieAcceptance.Accept;

        using var controller = new CookiesController(_cookieServiceMock.Object, _cookieOptions, _googleAnalyticsOptions)
        {
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),
            ControllerContext = _controllerContext
        };

        // Act
        var result = controller.Detail(returnUrl, cookiesAccepted) as ViewResult;

        // Assert
        _cookieServiceMock.Verify(cs => cs.SetCookieAcceptance(true, It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()));
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<CookieDetailViewModel>();
    }

    [TestMethod]
    public void UpdateAcceptance_SetsCookieAcceptanceAndRedirects()
    {
        // Arrange
        var returnUrl = "/epr-prn";
        var cookies = CookieAcceptance.Accept;

        using var controller = new CookiesController(_cookieServiceMock.Object, _cookieOptions, _googleAnalyticsOptions)
        {
            TempData = new TempDataDictionary(_controllerContext.HttpContext, Mock.Of<ITempDataProvider>()),
            ControllerContext = _controllerContext
        };

        // Act
        var result = controller.UpdateAcceptance(returnUrl, cookies) as LocalRedirectResult;

        // Assert
        _cookieServiceMock.Verify(cs => cs.SetCookieAcceptance(true, It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()));
        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);
    }

    [TestMethod]
    public void AcknowledgeAcceptance_RedirectsToReturnUrl()
    {
        // Arrange
        var returnUrl = "/epr-prn";

        using var controller = new CookiesController(_cookieServiceMock.Object, _cookieOptions, _googleAnalyticsOptions)
        {
            ControllerContext = _controllerContext
        };

        // Act
        var result = controller.AcknowledgeAcceptance(returnUrl) as LocalRedirectResult;

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);
    }
}