using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class BaseServiceTests<T> where T : class
{
    protected Mock<IEprFacadeServiceApiClient> MockFacadeClient { get; set; } = null!;

    protected NullLogger<T> NullLogger { get; set; } = null!;

    [TestInitialize]
    public void SetupEachTest()
    {
        MockFacadeClient = new Mock<IEprFacadeServiceApiClient>();
        NullLogger = new NullLogger<T>();
    }
}