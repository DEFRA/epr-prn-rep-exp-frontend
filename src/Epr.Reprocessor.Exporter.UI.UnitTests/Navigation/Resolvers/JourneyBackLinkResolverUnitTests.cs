using Epr.Reprocessor.Exporter.UI.Navigation;
using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation.Resolvers;

[TestClass]
public class JourneyBackLinkResolverUnitTests
{
    private class StubController : IBackLinkAwareController
    {
        public void StubAction() { }
        public bool TryGetBackLinkResolver(out IBackLinkResolver? resolver)
        {
            resolver = null;
            return false;
        }
    }

    [TestMethod]
    public async Task Returns_Empty_If_Journey_Is_Empty()
    {
        // Arrange
        var sessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var linkGenerator = new Mock<LinkGenerator>();
        var mockHttpContext = new Mock<HttpContext>();

        httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        sessionManager.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession { Journey = new List<string>() });

        var resolver = new JourneyBackLinkResolver(sessionManager.Object, httpContextAccessor.Object, linkGenerator.Object);
        var context = TestHelper.CreateContext(new object());
        
        // Act
        var result = await resolver.ResolveAsync(context);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task ResolveAsync_ReturnsEmpty_WhenBackLinkIsNull()
    {
        // Arrange
        var sessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var linkGenerator = new Mock<LinkGenerator>();
        var mockHttpContext = new Mock<HttpContext>();
        
        var session = new ReprocessorRegistrationSession
        {
            Journey = ["/step-one", "/step-two", "/step-three"]
        };

        mockHttpContext.Setup(o => o.Request.Path).Returns("/step-four");
        httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        sessionManager.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var context = TestHelper.CreateContext(new object());
        var resolver = new JourneyBackLinkResolver(sessionManager.Object, httpContextAccessor.Object, linkGenerator.Object);

        // Act
        var result = await resolver.ResolveAsync(context);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task ResolveAsync_ReturnBackLinkUrl()
    {
        // Arrange
        var sessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var linkGenerator = new Mock<LinkGenerator>();
        var mockHttpContext = new Mock<HttpContext>();

        var session = new ReprocessorRegistrationSession
        {
            Journey = ["/step-one", "/step-two", "/step-three"]
        };

        mockHttpContext.Setup(o => o.Request.Path).Returns("/step-three");
        httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        sessionManager.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var context = TestHelper.CreateContext(new StubController());
        var resolver = new JourneyBackLinkResolver(sessionManager.Object, httpContextAccessor.Object, linkGenerator.Object);

        // Act
        var result = await resolver.ResolveAsync(context);

        // Assert
        result.Should().Contain("/step-two");
    }
}