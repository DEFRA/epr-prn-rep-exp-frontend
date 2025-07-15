namespace Epr.Reprocessor.Exporter.UI.UnitTests.Middleware;

[TestClass]
public class SecurityHeaderMiddlewareUnitTests
{
    [TestMethod]
    public async Task Middleware_Invoke_AddsSecurityHeadersAndSetsNonce()
    {
        // Arrange: Create a mock request delegate that does nothing
        var mockNext = new Mock<RequestDelegate>();
        mockNext.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Arrange: Create a test HttpContext
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Arrange: Create mock configuration
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AzureAdB2C:Instance"]).Returns("https://login.microsoftonline.com");

        // Act: Instantiate and invoke the middleware
        var middleware = new SecurityHeaderMiddleware(mockNext.Object);
        await middleware.Invoke(context, configMock.Object);

        // Assert: Check nonce was set
        context.Items.Should().ContainKey(ContextKeys.ScriptNonceKey);
        context.Items[ContextKeys.ScriptNonceKey].Should().NotBeNull();

        // Assert: Validate that all expected headers are present
        var headers = context.Response.Headers;
        headers["Content-Security-Policy"].ToString().Should().Contain("script-src");
        headers["Cross-Origin-Embedder-Policy"].ToString().Should().Be("require-corp");
        headers["Cross-Origin-Opener-Policy"].ToString().Should().Be("same-origin");
        headers["Cross-Origin-Resource-Policy"].ToString().Should().Be("same-origin");
        headers["Permissions-Policy"].ToString().Should().Contain("camera");
        headers["Referrer-Policy"].ToString().Should().Be("strict-origin-when-cross-origin");
        headers["X-Content-Type-Options"].ToString().Should().Be("nosniff");
        headers["X-Frame-Options"].ToString().Should().Be("deny");
        headers["X-Permitted-Cross-Domain-Policies"].ToString().Should().Be("none");
        headers["X-Robots-Tag"].ToString().Should().Be("noindex, nofollow");
    }

    [TestMethod]
    public void GenerateNonce_ReturnsBase64String()
    {
        // Arrange
        var middleware = new SecurityHeaderMiddleware(_ => Task.CompletedTask);

        // Act
        var nonce = typeof(SecurityHeaderMiddleware)
            .GetMethod("GenerateNonce", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(middleware, null) as string;

        // Assert
        nonce.Should().NotBeNull();
        Convert.FromBase64String(nonce).Length.Should().Be(32);
    }

    [TestMethod]
    public void GetContentSecurityPolicyHeader_ReturnsCorrectFormat()
    {
        // Arrange
        var nonce = "abc123";
        var formActionAddress = "https://login.microsoftonline.com";

        // Act
        var policy = typeof(SecurityHeaderMiddleware)
            .GetMethod("GetContentSecurityPolicyHeader", BindingFlags.NonPublic | BindingFlags.Static)
            ?.Invoke(null, [nonce, formActionAddress]) as string;

        // Assert
        policy.Should().NotBeNull();
        policy.Should().Contain($"'nonce-{nonce}'");
        policy.Should().Contain($"form-action 'self' {formActionAddress}");
        policy.Should().Contain("default-src");
        policy.Should().Contain("img-src");
        policy.Should().Contain("connect-src");
    }
}