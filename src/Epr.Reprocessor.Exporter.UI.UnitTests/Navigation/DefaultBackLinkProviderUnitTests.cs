using Epr.Reprocessor.Exporter.UI.Navigation;
using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation;

[TestClass]
public class DefaultBackLinkProviderUnitTests
{
    [TestMethod]
    public async Task Uses_Controller_Resolver_If_Available()
    {
        // Arrange
        var resolverMock = new Mock<IBackLinkResolver>();
        resolverMock.Setup(r => r.ResolveAsync(It.IsAny<ActionExecutingContext>()))
            .ReturnsAsync("/custom-back");

        var controllerMock = new Mock<IBackLinkAwareController>();
        controllerMock.Setup(c => c.TryGetBackLinkResolver(out It.Ref<IBackLinkResolver>.IsAny!))
            .Callback(new TryGetBackLinkResolverCallback((out IBackLinkResolver resolver) => resolver = resolverMock.Object))
            .Returns(true);

        var context = TestHelper.CreateContext(controllerMock.Object);

        var provider = new DefaultBackLinkProvider(resolverMock.Object);

        // Act
        var result = await provider.GetBackLinkAsync(context);

        // Assert
        result.Should().BeEquivalentTo("/custom-back");
    }

    [TestMethod]
    public async Task Falls_Back_To_Default_Resolver_If_Not_Provided()
    {
        // Arrange
        var fallbackResolver = new Mock<IBackLinkResolver>();
        fallbackResolver.Setup(r => r.ResolveAsync(It.IsAny<ActionExecutingContext>()))
            .ReturnsAsync("/default-back");
        
        var nonAwareController = new object();
        var context = TestHelper.CreateContext(nonAwareController);

        var provider = new DefaultBackLinkProvider(fallbackResolver.Object);

        // Act
        var result = await provider.GetBackLinkAsync(context);

        // Assert
        result.Should().BeEquivalentTo("/default-back");
    }

    private delegate void TryGetBackLinkResolverCallback(out IBackLinkResolver resolver);
}