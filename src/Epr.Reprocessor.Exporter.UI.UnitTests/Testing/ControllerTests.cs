namespace Epr.Reprocessor.Exporter.UI.UnitTests.Testing;

public class ControllerTests<T> : TestBase where T : Controller
{
    protected ILogger<T> Logger => new NullLogger<T>();

    protected override void ConfigureServices(IServiceCollection serviceCollection)
    {
        base.ConfigureServices(serviceCollection);

        serviceCollection.AddTransient<T>();

        serviceCollection.AddScoped(_ => MockReprocessorService.Object);
        serviceCollection.AddScoped(_ => MockRegistrationService.Object);
        serviceCollection.AddScoped(_ => MockRegistrationMaterialService.Object);
        serviceCollection.AddScoped(_ => MockMaterialService.Object);
        serviceCollection.AddScoped(_ => MockRequestMapper.Object);
        serviceCollection.AddScoped(_ => MockPostcodeLookupService.Object);
        serviceCollection.AddScoped(_ => MockValidationService.Object);
        serviceCollection.AddScoped(_ => Logger);
    }
}