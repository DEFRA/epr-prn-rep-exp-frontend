using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services.ExporterJourney.Implementations
{
    [TestClass]
    public class WasteCarrierBrokerDealerRefServiceUnitTests
    {
        private Mock<IEprFacadeServiceApiClient> _apiClientMock;
        private Mock<ILogger<WasteCarrierBrokerDealerRefService>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _apiClientMock = new Mock<IEprFacadeServiceApiClient>();
            _loggerMock = new Mock<ILogger<WasteCarrierBrokerDealerRefService>>();
        }

        [TestMethod]
        public async Task GetByRegistrationId_CallsApiClientAndReturnsDto()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var expected = new WasteCarrierBrokerDealerRefDto { RegistrationId = registrationId };
            var httpResponse = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(expected))
            };

            _apiClientMock
                .Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(httpResponse);

            var service = new WasteCarrierBrokerDealerRefService(_apiClientMock.Object, _loggerMock.Object);

            // Act
            var result = await service.GetByRegistrationId(registrationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.RegistrationId, result.RegistrationId);
            _apiClientMock.Verify(x => x.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Save_CallsPostApiClient_WhenNoDtoIdGiven()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new WasteCarrierBrokerDealerRefDto { RegistrationId = registrationId };
            var httpResponse = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);

            _apiClientMock
                .Setup(x => x.SendPostRequest(It.IsAny<string>(), dto))
                .ReturnsAsync(httpResponse);

            var service = new WasteCarrierBrokerDealerRefService(_apiClientMock.Object, _loggerMock.Object);

            // Act
            await service.Save(dto);

            // Assert
            _apiClientMock.Verify(x => x.SendPostRequest(It.IsAny<string>(), dto), Times.Once);
        }

        [TestMethod]
        public async Task Save_CallsPutApiClient_WhenDtoIdGiven()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var carrierBrokerDealerPermitId = Guid.NewGuid();
            var dto = new WasteCarrierBrokerDealerRefDto 
            { 
                CarrierBrokerDealerPermitId = carrierBrokerDealerPermitId, 
                RegistrationId = registrationId 
            };
            var httpResponse = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);

            _apiClientMock
                .Setup(x => x.SendPutRequest(It.IsAny<string>(), dto))
                .ReturnsAsync(httpResponse);

            var service = new WasteCarrierBrokerDealerRefService(_apiClientMock.Object, _loggerMock.Object);

            // Act
            await service.Save(dto);

            // Assert
            _apiClientMock.Verify(x => x.SendPutRequest(It.IsAny<string>(), dto), Times.Once);
        }
    }
}