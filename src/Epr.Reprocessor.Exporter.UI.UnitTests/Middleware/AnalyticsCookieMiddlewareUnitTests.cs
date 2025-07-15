using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Middleware;

[TestClass]
public class AnalyticsCookieMiddlewareUnitTests : MiddlewareTestBase
{
    [TestMethod]
    public async Task InvokeAsync_SetsItemsAndCallsNextMiddleware()
    {
        // Arrange: Mock ICookieService
        var cookieServiceMock = new Mock<ICookieService>();
        cookieServiceMock
            .Setup(s => s.HasUserAcceptedCookies(It.IsAny<IRequestCookieCollection>()))
            .Returns(true);

        // Arrange: Setup IOptions<GoogleAnalyticsOptions>
        var analyticsOptions = Options.Create(new GoogleAnalyticsOptions
        {
            TagManagerContainerId = "GTM-XXXX"
        });

        SetupCookies(new Dictionary<string, string>{{ "cookieConsent", "true" }});

        // Act: Run middleware
        var middleware = new AnalyticsCookieMiddleware(MockNext.Object);
        await middleware.InvokeAsync(MockHttpContext, cookieServiceMock.Object, analyticsOptions);

        // Assert: Items are set
        MockHttpContext.Items[ContextKeys.UseGoogleAnalyticsCookieKey].Should().BeEquivalentTo(true);
        MockHttpContext.Items[ContextKeys.TagManagerContainerIdKey].Should().Be("GTM-XXXX");

        // Assert: Next delegate was called
        MockNext.Verify(n => n(MockHttpContext), Times.Once);
    }
}