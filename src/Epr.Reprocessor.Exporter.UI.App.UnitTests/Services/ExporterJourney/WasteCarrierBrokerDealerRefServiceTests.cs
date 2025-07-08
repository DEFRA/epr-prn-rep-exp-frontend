using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Implementations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services.ExporterJourney
{
    [ExcludeFromCodeCoverage]
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
        public async Task Save_SuccessfulRequest_CallsApiClientWithCorrectParameters()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = registrationId,
                WasteCarrierBrokerDealerRegistration = "REF-1232122"
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create("")
            };

            MockFacadeClient
                .Setup(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, registrationId), dto)).ReturnsAsync(httpResponse);

            // Act
            await _systemUnderTest.Save(dto);

            // Assert
            MockFacadeClient.Verify(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, registrationId), dto), Times.Once);
        }

        [TestMethod]
        public async Task Save_ApiClientReturnsError_ThrowsException()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new WasteCarrierBrokerDealerRefDto
            {
                RegistrationId = registrationId,
                WasteCarrierBrokerDealerRegistration = "REF-1232122"
            };

            MockFacadeClient
                .Setup(x => x.SendPostRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPost, Endpoints.CurrentVersion.Version, registrationId), dto))
                .Throws(new Exception());

            // Act & Assert
            await Assert.ThrowsExactlyAsync<Exception>(async () =>
            {
                await _systemUnderTest.Save(dto);
            });
        }

        [TestMethod]
        public async Task GetByRegistrationId_SuccessfulRequest_CallsApiClientWithCorrectParameters()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
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
        public async Task Update_SuccessfulRequest_CallsApiClientWithCorrectParameters()
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
            await _systemUnderTest.Update(dto);

            // Assert
            MockFacadeClient.Verify(x => x.SendPutRequest(string.Format(Endpoints.ExporterJourney.WasteCarrierBrokerDealerRefPut, Endpoints.CurrentVersion.Version, dto.RegistrationId), dto), Times.Once);
        }

        [TestMethod]
        public async Task Update_ApiClientReturnsError_ThrowsException()
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
                await _systemUnderTest.Update(dto);
            });
        }

    }
}
