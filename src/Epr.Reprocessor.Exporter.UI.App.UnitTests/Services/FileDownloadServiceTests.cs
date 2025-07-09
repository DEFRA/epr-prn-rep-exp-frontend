using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class FileDownloadServiceTests
{
    private Mock<IWebApiGatewayClient> _webApiGatewayClientMock = null!;
    private FileDownloadService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _webApiGatewayClientMock = new Mock<IWebApiGatewayClient>();
        _service = new FileDownloadService(_webApiGatewayClientMock.Object);
    }

    [TestMethod]
    public async Task GetFileAsync_ReturnsByteArray_WhenDownloadSuccessful()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var fileName = "file.txt";
        var submissionType = SubmissionType.Accreditation;
        var submissionId = Guid.NewGuid();
        var rnd = new Random();
        byte[] data = new byte[10];
        rnd.NextBytes(data);

        _webApiGatewayClientMock.Setup(service => service.FileDownloadAsync(It.IsAny<string>())).ReturnsAsync(data);

        // Act
        var result = await _service.GetFileAsync(fileId, fileName, submissionType, submissionId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Equal(data);
    }

    [TestMethod]
    public async Task GetFileAsync_ThrowsExceptions_WhenDownloadIsNotSuccessful()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var fileName = "file.txt";
        var submissionType = SubmissionType.Accreditation;
        var submissionId = Guid.NewGuid();
        
        _webApiGatewayClientMock.Setup(service => service.FileDownloadAsync(It.IsAny<string>())).Throws(new Exception("internal server error occurred"));

        // Act
        Func<Task> action = async () => await _service.GetFileAsync(fileId, fileName, submissionType, submissionId);

        // Assert
        await action.Should().ThrowAsync<Exception>("an internal server error occurred");
    }
}
