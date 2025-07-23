using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class ExporterRegistrationServiceTests
    {
        private Mock<IEprFacadeServiceApiClient> _mockClient = null!;
        private Mock<ILogger<ExporterRegistrationService>> _mockLogger = null!;
        private ExporterRegistrationService _service = null!;
        private Fixture _fixture = null!;
        private JsonSerializerOptions _options = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockClient = new Mock<IEprFacadeServiceApiClient>();
            _mockLogger = new Mock<ILogger<ExporterRegistrationService>>();
            _service = new ExporterRegistrationService(_mockClient.Object, _mockLogger.Object);
            _fixture = new Fixture();
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
        }

        [TestMethod]
        public async Task SaveOverseasReprocessorAsync_CallsClientAndSucceeds()
        {
            // Arrange
            var request = _fixture.Create<OverseasAddressRequestDto>();
            _mockClient.Setup(x => x.SendPostRequest(It.IsAny<string>(), request))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _service.SaveOverseasReprocessorAsync(request);

            // Assert
            _mockClient.Verify(x => x.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task SaveOverseasReprocessorAsync_ThrowsAndLogs_WhenHttpRequestException()
        {
            // Arrange
            var request = _fixture.Create<OverseasAddressRequestDto>();
            var exception = new HttpRequestException("fail");
            _mockClient.Setup(x => x.SendPostRequest(It.IsAny<string>(), request))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _service.SaveOverseasReprocessorAsync(request);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to save the overseas reprocessor")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetOverseasMaterialReprocessingSites_ReturnsList_WhenApiReturnsSuccess()
        {
            // Arrange
            var registrationMaterialId = Guid.NewGuid();
            var expected = _fixture.CreateMany<OverseasMaterialReprocessingSiteDto>(2).ToList();
            var json = JsonSerializer.Serialize(expected, _options);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _service.GetOverseasMaterialReprocessingSites(registrationMaterialId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetOverseasMaterialReprocessingSites_ReturnsNull_WhenStatusCodeIsNotFound()
        {
            // Arrange
            var registrationMaterialId = Guid.NewGuid();
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _service.GetOverseasMaterialReprocessingSites(registrationMaterialId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetOverseasMaterialReprocessingSites_ReturnsNull_WhenHttpRequestExceptionWithNotFound()
        {
            // Arrange
            var registrationMaterialId = Guid.NewGuid();
            var exception = new HttpRequestException("Not found", null, HttpStatusCode.NotFound);
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ThrowsAsync(exception);

            // Act
            var result = await _service.GetOverseasMaterialReprocessingSites(registrationMaterialId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetOverseasMaterialReprocessingSites_ThrowsAndLogs_WhenHttpRequestExceptionOther()
        {
            // Arrange
            var registrationMaterialId = Guid.NewGuid();
            var exception = new HttpRequestException("fail", null, HttpStatusCode.InternalServerError);
            _mockClient.Setup(x => x.SendGetRequest(It.IsAny<string>())).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _service.GetOverseasMaterialReprocessingSites(registrationMaterialId);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to get overseas material reprocessing Sites")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpsertInterimSitesAsync_CallsClientAndEnsuresSuccess()
        {
            // Arrange
            var request = _fixture.Create<SaveInterimSitesRequestDto>();
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            _mockClient.Setup(x => x.SendPostRequest(It.IsAny<string>(), request)).ReturnsAsync(response);

            // Act
            await _service.UpsertInterimSitesAsync(request);

            // Assert
            _mockClient.Verify(x => x.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task UpsertInterimSitesAsync_ThrowsAndLogs_WhenHttpRequestException()
        {
            // Arrange
            var request = _fixture.Create<SaveInterimSitesRequestDto>();
            var exception = new HttpRequestException("fail");
            _mockClient.Setup(x => x.SendPostRequest(It.IsAny<string>(), request)).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _service.UpsertInterimSitesAsync(request);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to save the interim sites")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}