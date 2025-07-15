using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services.ExporterJourney.Implementations
{
    [TestClass]
    public class CheckYourAnswersForNoticeAddressServiceUnitTests
    {
        private Mock<IEprFacadeServiceApiClient> _apiClientMock;
        private Mock<ILogger<CheckYourAnswersForNoticeAddressService>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _apiClientMock = new Mock<IEprFacadeServiceApiClient>();
            _loggerMock = new Mock<ILogger<CheckYourAnswersForNoticeAddressService>>();
        }

        [TestMethod]
        public async Task GetByRegistrationId_CallsApiClientAndReturnsDto()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var expected = new AddressDto { AddressLine1 = "Some address line 1" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expected))
            };

            _apiClientMock
                .Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(httpResponse);

            var service = new CheckYourAnswersForNoticeAddressService(_apiClientMock.Object, _loggerMock.Object);

            // Act
            var result = await service.GetByRegistrationId(registrationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.AddressLine1, result.AddressLine1);
            _apiClientMock.Verify(x => x.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Save_CallsApiClientWithDto()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new AddressDto { AddressLine1 = "Some address line 1" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            _apiClientMock
                .Setup(x => x.SendPutRequest(It.IsAny<string>(), dto))
                .ReturnsAsync(httpResponse);

            var service = new CheckYourAnswersForNoticeAddressService(_apiClientMock.Object, _loggerMock.Object);

            // Act
            await service.Save(registrationId, dto);

            // Assert
            _apiClientMock.Verify(x => x.SendPutRequest(It.IsAny<string>(), dto), Times.Once);
        }
    }
}
