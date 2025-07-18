using Epr.Reprocessor.Exporter.UI.Navigation;
using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation.Filters;

[TestClass]
public class IBackLinkAwareControllerUnitTests
{
    private class TestController : IBackLinkAwareController
    {
        public bool TryGetBackLinkResolver(out IBackLinkResolver? resolver)
        {
            resolver = null;
            return false;
        }
    }

    [TestMethod]
    public async Task OnBackHandlerAsync_DefaultImplementation()
    {
        // Arrange
        var controller = new TestController() as IBackLinkAwareController;

        // Act
        var result = await controller.OnBackHandlerAsync("");

        // Assert
        result.Should().BeOfType<EmptyResult>();
    }
}