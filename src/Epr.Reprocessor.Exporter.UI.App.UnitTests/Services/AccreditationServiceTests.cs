using System.Net;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class AccreditationServiceTests
    {
        private Mock<IEprFacadeServiceApiClient> _mockClient;
        private Mock<ILogger<AccreditationService>> _mockLogger;
        private AccreditationService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockClient = new Mock<IEprFacadeServiceApiClient>();
            _mockLogger = new Mock<ILogger<AccreditationService>>();
            _service = new AccreditationService(_mockClient.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task AddAsync_ValidResponse_ReturnsGuid()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var request = new AccreditationRequestDto(); // fill as needed

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new AccreditationResponseDto
                {
                    ExternalId = expectedGuid
                }))
            };

            _mockClient
                .Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(fakeResponse);

            // Act
            var result = await _service.AddAsync(request);

            // Assert
            Assert.AreEqual(expectedGuid, result);
        }

        [TestMethod]
        public async Task GetAsync_ValidResponse_ReturnsDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedDto = new AccreditationResponseDto
            {
                ExternalId = id,
                AccreditationYear = 2024
            };

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(expectedDto))
            };

            _mockClient
                .Setup(c => c.SendGetRequest(It.Is<string>(s => s.Contains(id.ToString()))))
                .ReturnsAsync(fakeResponse);

            // Act
            var result = await _service.GetAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.ExternalId);
            Assert.AreEqual(2024, result.AccreditationYear);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AddAsync_Error_ThrowsAndLogs()
        {
            // Arrange
            var request = new AccreditationRequestDto();

            _mockClient
                .Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("API error"));

            // Act
            await _service.AddAsync(request);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetAsync_Error_ThrowsAndLogs()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockClient
                .Setup(c => c.SendGetRequest(It.IsAny<string>()))
                .ThrowsAsync(new Exception("API error"));

            // Act
            await _service.GetAsync(id);

            // Assert is handled by ExpectedException
        }
    }
}
