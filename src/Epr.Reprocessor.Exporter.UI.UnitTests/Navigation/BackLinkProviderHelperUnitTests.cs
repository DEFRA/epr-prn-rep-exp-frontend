using Epr.Reprocessor.Exporter.UI.Navigation;
using Epr.Reprocessor.Exporter.UI.Navigation.Extensions;
using Epr.Reprocessor.Exporter.UI.Navigation.Resolvers;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation;

[TestClass]
public class BackLinkProviderHelperUnitTests
{
    [TestMethod]
    public void GetActionName_Returns_Handler_For_Implementing_Controller()
    {
        // Arrange
        var controller = new TestBackLinkAwareController();

        // Act
        var result = BackLinkProviderHelper.GetActionName(controller);

        // Assert
        result.Should().BeEquivalentTo("on-back-handler");
    }

    [TestMethod]
    public void GetActionName_Throws_If_Controller_Not_Implementing_Interface()
    {
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => BackLinkProviderHelper.GetActionName(new object()));
    }

    private class TestBackLinkAwareController : IBackLinkAwareController
    {
        public bool TryGetBackLinkResolver(out IBackLinkResolver? resolver)
        {
            resolver = null;
            return false;
        }
    }
}