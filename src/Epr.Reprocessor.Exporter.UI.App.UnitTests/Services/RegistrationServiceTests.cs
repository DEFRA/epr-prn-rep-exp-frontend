using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Services
{
    [TestClass]
    public class RegistrationServiceTests
    {
        private Mock<IEprFacadeServiceApiClient> _mockClient = null!;
        private Mock<ILogger<RegistrationService>> _mockLogger = null!;
        private RegistrationService _service = null!;
        private Fixture _fixture = null!;
        JsonSerializerOptions options = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockClient = new Mock<IEprFacadeServiceApiClient>();
            _mockLogger = new Mock<ILogger<RegistrationService>>();
            _service = new RegistrationService(_mockClient.Object, _mockLogger.Object);
            _fixture = new Fixture();
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
        }

        [TestMethod]
        public async Task GetCountries_ReturnsCountries_WhenApiReturnsSuccess()
        {
            // Arrange
            var countries = new List<string> { "UK", "France", "Germany" };
            var json = JsonSerializer.Serialize(countries, options);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _service.GetCountries();

            // Assert
            result.Should().BeEquivalentTo(countries);
        }

        [TestMethod]
        public async Task GetCountries_ThrowsException_AndLogsError_WhenApiFails()
        {
            // Arrange
            var exception = new Exception("API error");
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _service.GetCountries();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API error");
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to Get Countries")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task GetCountries_ThrowsException_AndLogsError_WhenStatusCodeIsNotSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            Func<Task> act = async () => await _service.GetCountries();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to Get Countries")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
