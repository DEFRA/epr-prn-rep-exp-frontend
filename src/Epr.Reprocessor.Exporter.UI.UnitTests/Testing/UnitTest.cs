namespace Epr.Reprocessor.Exporter.UI.UnitTests.Testing;

public class UnitTest
{
    private IServiceCollection _serviceCollection = null!;

    protected Mock<IRequestMapper> MockRequestMapper { get; set; } = null!;

    protected Mock<IReprocessorService> MockReprocessorService { get; set; } = null!;

    protected Mock<IPostcodeLookupService> MockPostcodeLookupService { get; set; } = null!;

    protected Mock<IMaterialService> MockMaterialService { get; set; } = null!;

    protected Mock<IRegistrationMaterialService> MockRegistrationMaterialService { get; set; } = null!;

    protected Mock<IValidationService> MockValidationService { get; set; } = null!;

    protected Mock<IRegistrationService> MockRegistrationService { get; set; } = null!;

    protected IServiceProvider Services => _serviceCollection.BuildServiceProvider();

    [TestInitialize]
    public void Setup()
    {
        _serviceCollection = new ServiceCollection();
        ConfigureMocks();
        ConfigureServices(_serviceCollection);
    }

    protected virtual void ConfigureServices(IServiceCollection serviceCollection)
    {
    }

    protected virtual void ConfigureMocks()
    {
        MockRequestMapper = new Mock<IRequestMapper>();
        MockReprocessorService = new Mock<IReprocessorService>();
        MockPostcodeLookupService = new Mock<IPostcodeLookupService>();
        MockMaterialService = new Mock<IMaterialService>();
        MockRegistrationMaterialService = new Mock<IRegistrationMaterialService>();
        MockValidationService = new Mock<IValidationService>();
        MockRegistrationService = new Mock<IRegistrationService>();
    }
}