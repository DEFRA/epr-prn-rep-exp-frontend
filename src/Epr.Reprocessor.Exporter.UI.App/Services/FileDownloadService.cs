using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class FileDownloadService(IWebApiGatewayClient webApiGatewayClient) : IFileDownloadService
{
    private readonly IWebApiGatewayClient _webApiGatewayClient = webApiGatewayClient;

    public async Task<byte[]?> GetFileAsync(
        Guid fileId,
        string fileName,
        SubmissionType submissionType,
        Guid submissionId)
    {
        var queryString = $"fileName={fileName}"
            + $"&fileid={fileId}"
            + $"&submissiontype={submissionType}"
            + $"&submissionid={submissionId}";

        return await _webApiGatewayClient.FileDownloadAsync(queryString);
    }
}
