using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Moq.Protected;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class WebApiGatewayClientTests
{
    private readonly Mock<ITokenAcquisition> _tokenAcquisitionMock = new();
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<ILogger<WebApiGatewayClient>> _loggerMock;
    private WebApiGatewayClient _webApiGatewayClient;
    private HttpClient _httpClient;
    private static readonly IFixture _fixture = new Fixture();

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<WebApiGatewayClient>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClient.BaseAddress = new Uri("https://test.com/");
        _webApiGatewayClient = new WebApiGatewayClient(
            _loggerMock.Object,
            _httpClient,
            _tokenAcquisitionMock.Object,
            Microsoft.Extensions.Options.Options.Create(new WebApiOptions { DownstreamScope = "https://api.com", BaseEndpoint = "https://test.com/" })
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient.Dispose();
    }

    [TestMethod]
    public async Task UploadFileAccreditationAsync_Should_Process_Successfully()
    {
        // Arrange
        var byteArray = Array.Empty<byte>();
        var fileName = "file.txt";
        var submissionType = SubmissionType.Accreditation;
        var submissionId = Guid.NewGuid();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Headers = { { "Location", $"/{submissionId}" } }
            });

        // Act
        var result = await _webApiGatewayClient.UploadFileAccreditationAsync(byteArray, fileName, submissionType, null);

        // Assert
        result.Should().Be(submissionId);
    }

    [TestMethod]
    public async Task UploadFileAccreditationAsync_ShouldSetHttpHeaders()
    {
        // Arrange
        var byteArray = Array.Empty<byte>();
        var fileName = "file.txt";
        var submissionType = SubmissionType.Accreditation;
        var submissionId = Guid.NewGuid();
        HttpRequestHeaders headers = _httpClient.DefaultRequestHeaders;
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Headers = { { "Location", $"/{submissionId}" } }
            });

        // Act
        var result = await _webApiGatewayClient.UploadFileAccreditationAsync(byteArray, fileName, submissionType, null);

        // Assert
        headers.GetValues("FileName").Single().Should().Be(fileName);
        headers.GetValues("SubmissionType").Single().Should().Be(submissionType.ToString());
    }

    [TestMethod]
    public async Task UploadFileAccreditationAsync_ThrowsExceptions_WhenUploadIsNotSuccessful()
    {
        // Arrange
        var byteArray = Array.Empty<byte>();
        var fileName = "file.txt";
        var submissionType = SubmissionType.Accreditation;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

        // Act
        Func<Task> action = async () => await _webApiGatewayClient.UploadFileAccreditationAsync(byteArray, fileName, submissionType, null);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [TestMethod]
    public async Task GetSubmissionAsync_ReturnsSubmission_WhenSubmissionExists()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var fileName = "file.txt";
        
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new AccreditationSubmission { Id = submissionId, FileId = fileId, AccreditationFileName = fileName }))
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _webApiGatewayClient.GetSubmissionAsync<AccreditationSubmission>(submissionId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(submissionId);
        result.FileId.Should().Be(fileId);
        result.AccreditationFileName.Should().Be(fileName);
    }

    [TestMethod]
    public async Task GetSubmissionAsync_ReturnsNull_WhenResponseCodeIsNotFound()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

        // Act
        var result = await _webApiGatewayClient.GetSubmissionAsync<AccreditationSubmission>(submissionId);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetSubmissionAsync_ThrowsException_WhenResponseCodeIsNotSuccessfulOr404()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

        // Act / Assert
        Func<Task> action = async () => await _webApiGatewayClient.GetSubmissionAsync<AccreditationSubmission>(submissionId);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>("");
    }

    [TestMethod]
    public async Task FileDownloadAsync_ShouldReturnFileData_WhenResponseIsSuccessful()
    {
        // Arrange
        Random rnd = new();
        byte[] data = new byte[10];
        rnd.NextBytes(data);
        var expectedData = data;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedData))
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _webApiGatewayClient.FileDownloadAsync(It.IsAny<string>());

        // Assert
        result.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task FileDownloadAsync_ShouldThrowException_WhenResponseIsNotSuccessful()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        Func<Task> action = async () => await _webApiGatewayClient.FileDownloadAsync(It.IsAny<string>());

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>("an internal server error occurred");
    }
}
