using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation.Resolvers;

[TestClass]
public class JourneyBackLinkResolverUnitTests
{
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
}