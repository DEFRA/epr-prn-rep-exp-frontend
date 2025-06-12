using System.Net.Http.Json;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class SaveAndContinueServiceTests
    {
        private Mock<IEprFacadeServiceApiClient> _mockClient;
        private Mock<ILogger<SaveAndContinueService>> _mockLogger;
        private SaveAndContinueService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockClient = new Mock<IEprFacadeServiceApiClient>();
            _mockLogger = new Mock<ILogger<SaveAndContinueService>>();
            _service = new SaveAndContinueService(_mockClient.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task AddAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new SaveAndContinueRequestDto();
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<SaveAndContinueRequestDto>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.AddAsync(request)); 
        }

        [TestMethod]
        public async Task AddAsync_ShouldCallSendPostRequest()
        {
            // Arrange
            var request = new SaveAndContinueRequestDto();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("")
            };
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<SaveAndContinueRequestDto>()))
                       .ReturnsAsync(response);

            // Act
            await _service.AddAsync(request);

            // Assert
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task GetLatestAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var registrationId = 1;
            var controller = "TestController";
            var area = "TestArea";
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _service.GetLatestAsync(registrationId, controller, area);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetLatestAsync_ShouldReturnResponseDto_WhenSuccess()
        {
            // Arrange
            var registrationId = 1;
            var controller = "TestController";
            var area = "TestArea";
            var responseDto = new SaveAndContinueResponseDto();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(responseDto)
            };
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _service.GetLatestAsync(registrationId, controller, area);

            // Assert
            Assert.IsNotNull(result); 
        }

        [TestMethod]
        public async Task GetLatestAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var registrationId = 1;
            var controller = "TestController";
            var area = "TestArea";
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.GetLatestAsync(registrationId, controller, area);

            // Assert
            Assert.IsNull(result); 
        }
    }
}
