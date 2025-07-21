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
}