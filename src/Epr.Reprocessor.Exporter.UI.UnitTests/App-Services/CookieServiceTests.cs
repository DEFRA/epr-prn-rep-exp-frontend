using Epr.Reprocessor.Exporter.UI.App.Services;

namespace Epr.Reprocessor.Exporter.UI.App.Tests.Services
{
    [TestClass]
    public class CookieServiceTests
    {
        private Mock<ILogger<CookieService>> _loggerMock;
        private Mock<IOptions<Options.CookieOptions>> _cookieOptionsMock;
        private Mock<IOptions<GoogleAnalyticsOptions>> _googleAnalyticsOptionsMock;
        private CookieService _cookieService;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CookieService>>();
            _cookieOptionsMock = new Mock<IOptions<Options.CookieOptions>>();
            _googleAnalyticsOptionsMock = new Mock<IOptions<GoogleAnalyticsOptions>>();

            _cookieOptionsMock.Setup(o => o.Value).Returns(new Options.CookieOptions
            {
                CookiePolicyCookieName = "TestCookiePolicy",
                CookiePolicyDurationInMonths = 12
            });

            _googleAnalyticsOptionsMock.Setup(o => o.Value).Returns(new GoogleAnalyticsOptions
            {
                CookiePrefix = "GA-"
            });

            _cookieService = new CookieService(
                _loggerMock.Object,
                _cookieOptionsMock.Object,
                _googleAnalyticsOptionsMock.Object);
        }

        [TestMethod]
        public void SetCookieAcceptance_ShouldSetCookie_WhenAccepted()
        {
            // Arrange
            var responseCookiesMock = new Mock<IResponseCookies>();
            var cookiesMock = new Mock<IRequestCookieCollection>();

            // Act
            _cookieService.SetCookieAcceptance(true, cookiesMock.Object, responseCookiesMock.Object);

            // Assert
            responseCookiesMock.Verify(r => r.Append(
                "TestCookiePolicy",
                "True",
                It.Is<Microsoft.AspNetCore.Http.CookieOptions>(o => o.Expires.HasValue && o.Expires.Value > DateTimeOffset.UtcNow)),
                Times.Once);
        }

        [TestMethod]
        public void SetCookieAcceptance_ShouldExpireGoogleAnalyticsCookies_WhenNotAccepted()
        {
            // Arrange
            var responseCookiesMock = new Mock<IResponseCookies>();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.GetEnumerator()).Returns(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("GA-Test1", "Value1"),
                new KeyValuePair<string, string>("GA-Test2", "Value2")
            }.GetEnumerator());

            // Act
            _cookieService.SetCookieAcceptance(false, cookiesMock.Object, responseCookiesMock.Object);

            // Assert
            responseCookiesMock.Verify(r => r.Append(
                It.Is<string>(key => key.StartsWith("GA-")),
                It.IsAny<string>(),
                It.Is<Microsoft.AspNetCore.Http.CookieOptions>(o => o.Expires.HasValue && o.Expires.Value < DateTimeOffset.UtcNow)),
                Times.Exactly(2));
        }

        [TestMethod]
        public void HasUserAcceptedCookies_ShouldReturnTrue_WhenCookieExistsAndIsTrue()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["TestCookiePolicy"]).Returns("true");

            // Act
            var result = _cookieService.HasUserAcceptedCookies(cookiesMock.Object);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasUserAcceptedCookies_ShouldReturnFalse_WhenCookieDoesNotExist()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["TestCookiePolicy"]).Returns((string)null);

            // Act
            var result = _cookieService.HasUserAcceptedCookies(cookiesMock.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasUserAcceptedCookies_ShouldReturnFalse_WhenCookieValueIsInvalid()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["TestCookiePolicy"]).Returns("invalid");

            // Act
            var result = _cookieService.HasUserAcceptedCookies(cookiesMock.Object);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
