using EPR.Common.Authorization.Sessions;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.Tests;

[TestClass]
public class ServiceProviderExtensionTests
{
	private Mock<IServiceCollection> _servicesMock;
	private Mock<IConfiguration> _configurationMock;
	[TestInitialize]
	public void Setup()
	{
		_servicesMock = new Mock<IServiceCollection>();
		_configurationMock = new Mock<IConfiguration>();
	}

	[TestMethod]
	public void RegisterWebComponents_ShouldRegisterAllComponents()
	{
		// Arrange
		var services = new ServiceCollection();
		// Mock IConfiguration with a valid configuration
		var configurationData = new Dictionary<string, string>
		{
			{ "CookieOptions:ConfigSection", "SomeValue" },
			{ "MsalOptions:ConfigSection", "SomeValue" },
			{ "SessionOptions:ConfigSection", "SomeValue" },
			{ "RedisOptions:ConfigSection", "SomeValue" }
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(configurationData)
			.Build();
		// Act
		ServiceProviderExtension.RegisterWebComponents(services, configuration);
		// Assert
		Assert.IsTrue(services.Any(s => s.ServiceType == typeof(ICookieService)));
		Assert.IsTrue(services.Any(s => s.ServiceType == typeof(ISaveAndContinueService)));
		Assert.IsTrue(services.Any(s => s.ServiceType == typeof(ISessionManager<ReprocessorExporterRegistrationSession>)));
		// Add more assertions for other registrations
	}


	[TestMethod]
	public void ConfigureMsalDistributedTokenOptions_ShouldConfigureOptions()
	{
		// Arrange
		var services = new ServiceCollection();
		// Act
		ServiceProviderExtension.ConfigureMsalDistributedTokenOptions(services);
		// Assert
		var options = services.BuildServiceProvider().GetService<IOptions<MsalDistributedTokenCacheAdapterOptions>>();
		Assert.IsNotNull(options);
		// Add more assertions for specific configurations
	}

	// Add similar tests for other methods
}