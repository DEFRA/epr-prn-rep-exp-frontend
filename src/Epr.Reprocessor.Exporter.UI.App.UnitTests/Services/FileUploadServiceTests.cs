using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class FileUploadServiceTests
{
    private Mock<IWebApiGatewayClient> _webApiGatewayClientMock = null!;
    private FileUploadService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _webApiGatewayClientMock = new Mock<IWebApiGatewayClient>();
        _service = new FileUploadService(_webApiGatewayClientMock.Object);
    }

    [TestMethod]
    public async Task UploadFileAccreditationAsync_ShouldReturn_SubmissionId_FromApiClient()
    {
        // Arrange
        byte[] fileContent = [];
        var fileName = "file1.txt";
        var submissionType = SubmissionType.Accreditation;
        Guid? submissionId = null;
        var expectedSubmissionId = Guid.NewGuid();

        _webApiGatewayClientMock
            .Setup(client => client.UploadFileAccreditationAsync(fileContent, fileName, submissionType, submissionId))
            .ReturnsAsync(expectedSubmissionId);

        // Act
        var result = await _service.UploadFileAccreditationAsync(fileContent, fileName, submissionType, submissionId);

        // Assert
        result.Should().Be(expectedSubmissionId);
        _webApiGatewayClientMock.Verify(client => client.UploadFileAccreditationAsync(fileContent, fileName, submissionType, submissionId), Times.Once);
    }

    [TestMethod]
    public async Task GetFileUploadStatusAsync_CallsClient_WhenCalled()
    {
        // Arrange
        var submissionId = Guid.NewGuid();

        // Act
        await _service.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId);

        // Assert
        _webApiGatewayClientMock.Verify(x => x.GetSubmissionAsync<AccreditationSubmission>(submissionId), Times.Once);
    }
}
