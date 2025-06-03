using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Moq.Protected;
using Moq;
using System.Net;
using System.Text.Json;
using FrontendSchemeRegistration.Application.Services.ExporterJourney.Implementations;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services.ExporterJourney
{

    [TestFixture]
    public class ExporterJourneyWebApiGatewayClientTests
    {
        private const string SubmissionPeriod = "Jun to Dec 23";
        private readonly Mock<ITokenAcquisition> _tokenAcquisitionMock = new();
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<ILogger<ExporterJourneyWebApiGatewayClient>> _loggerMock;
        private ExporterJourneyWebApiGatewayClient _webApiGatewayClient;
        private HttpClient _httpClient;
        private static readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<ExporterJourneyWebApiGatewayClient>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _webApiGatewayClient = new FrontendSchemeRegistration.Application.Services.ExporterJourney.Implementations.ExporterJourneyWebApiGatewayClient(
                _httpClient,
                _tokenAcquisitionMock.Object,
                Options.Create(new HttpClientOptions { UserAgent = "SchemeRegistration/1.0" }),
                Options.Create(new WebApiOptions { DownstreamScope = "https://api.com", BaseEndpoint = "https://example.com/" }),
                _loggerMock.Object
                );
        }


        [TearDown]
        public void Teardown()
        {
            _httpClient.Dispose();
        }


        [Test]
        public async Task Get_ReturnsObject_WhenCorrectUriGiven()
        {
            // Arrange
            var id = Guid.NewGuid();
            var uri = "some uri";
            var responseContent = "some response";

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responseContent))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _webApiGatewayClient.Get<string>(id, uri);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(responseContent);
        }

        [Test]
        public async Task Get_ReturnsNotFoundStatus_WhenInCorrectUriGiven()
        {
            // Arrange
            var id = Guid.NewGuid();
            var uri = "some uri";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _webApiGatewayClient.Get<string>(id, uri);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task Get_ThrowsException_WhenResponseCodeIsNotSuccessfulOr404()
        {
            // Arrange
            var id = Guid.NewGuid();
            var uri = "some uri";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

            // Act / Assert
            await _webApiGatewayClient
                .Invoking(x => x.Get<string>(id, uri))
                .Should()
                .ThrowAsync<HttpRequestException>();
            _loggerMock.VerifyLog(x => x.LogError($"Error getting data for id: {id}"));
        }
    }
}
