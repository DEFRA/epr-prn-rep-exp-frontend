using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;
using System.Net.Http.Json;
using System.Text.Json;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services.ExporterJourney
{
    [TestClass]
    public class WasteCarrierBrokerDealerRefServiceTests : BaseServiceTests<WasteCarrierBrokerDealerRefService>
    {
        private WasteCarrierBrokerDealerRefService _systemUnderTest = null!;
        private JsonSerializerOptions _serializerOptions = null!;

        [TestInitialize]
        public void Setup()
        {
            SetupEachTest();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _systemUnderTest = new WasteCarrierBrokerDealerRefService(MockFacadeClient.Object, NullLogger);
        }

        [TestMethod]
        public async Task SaveAync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
        {
            // Arrange
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = Guid.NewGuid(),
                WasteCarrierBrokerDealerRegistration = "REF-1232122"
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create("")
            };

            MockFacadeClient
                .Setup(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto)).ReturnsAsync(httpResponse);

            // Act
            await _systemUnderTest.SaveAsync(dto);

            // Assert
            MockFacadeClient.Verify(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto), Times.Once);
        }

        [TestMethod]
        public async Task SaveAsync_ApiClientReturnsError_ThrowsException()
        {
            // Arrange
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = Guid.NewGuid(),
                WasteCarrierBrokerDealerRegistration = "REF-1232122"
            };

            MockFacadeClient
                             .Setup(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto))
                .Throws(new Exception());

            // Act & Assert
            await Assert.ThrowsExactlyAsync<Exception>(async () =>
            {
                await _systemUnderTest.SaveAsync(dto);
            });
        }

        [TestMethod]
        public async Task GetByRegistrationId_SuccessfulRequest_CallsApiClientWithCorrectParameters()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var wasteCarrierBrokerDealerRefServiceDto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = registrationId,
                 CarrierBrokerDealerPermitId = Guid.NewGuid(),
                  RegisteredWasteCarrierBrokerDealerFlag= false,
                   WasteCarrierBrokerDealerRegistration = "REF-2124552"
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(wasteCarrierBrokerDealerRefServiceDto, _serializerOptions))
            };

            // Expectations
            MockFacadeClient
                .Setup(x => x.SendGetRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefGet, Endpoints.CurrentVersion.Version, registrationId)))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.GetByRegistrationId(registrationId);

            // Assert
            result.Should().BeEquivalentTo(wasteCarrierBrokerDealerRefServiceDto);
        }

        [TestMethod]
        public async Task UpdateAync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
        {
            // Arrange
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = Guid.NewGuid()
            };

            var responseDto = new WasteCarrierBrokerDealerRefDto();
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(responseDto)
            };

            MockFacadeClient
                .Setup(x => x.SendPutRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPut, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto)).ReturnsAsync(httpResponse);

            // Act
            await _systemUnderTest.UpdateAsync(dto);

            // Assert
            MockFacadeClient.Verify(x => x.SendPutRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPut, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAync_ApiClientReturnsError_ThrowsException()
        {
            // Arrange
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = Guid.NewGuid()
            };

            MockFacadeClient
                .Setup(x => x.SendPutRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPut, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto))
                .Throws(new Exception());

            // Act & Assert
            await Assert.ThrowsExactlyAsync<Exception>(async () =>
            {
                await _systemUnderTest.UpdateAsync(dto);
            });
        }

    }
}
