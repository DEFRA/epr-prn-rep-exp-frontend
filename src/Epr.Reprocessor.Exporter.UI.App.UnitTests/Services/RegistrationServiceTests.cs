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
            new JsonSerializerOptions
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

        [TestMethod]
        public async Task GetRegistrationsOverviewByOrgIdAsync_EmptyGuid_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetRegistrationsOverviewByOrgIdAsync(Guid.Empty);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetRegistrationsOverviewByOrgIdAsync_NotFoundStatus_ReturnsEmptyList()
        {
            // Arrange
            var orgId = Guid.NewGuid();
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _service.GetRegistrationsOverviewByOrgIdAsync(orgId);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetRegistrationsOverviewByOrgIdAsync_ValidResponse_ReturnsRegistrations()
        {
            // Arrange
            var orgId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var registrations = new List<RegistrationOverviewDto>
            {
                new()  { RegistrationId = id },
                new() { RegistrationId = Guid.NewGuid() }
            };

            var content = new StringContent(JsonSerializer.Serialize(registrations, options));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _service.GetRegistrationsOverviewByOrgIdAsync(orgId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(x => x.RegistrationId == id);
        }

        [TestMethod]
        public async Task GetRegistrationsOverviewByOrgIdAsync_ThrowsException_LogsAndRethrows()
        {
            // Arrange
            var orgId = Guid.NewGuid();
            var exception = new HttpRequestException("Internal server error");

            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _service.GetRegistrationsOverviewByOrgIdAsync(orgId);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Failed to get registration overview data")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

    }
}
