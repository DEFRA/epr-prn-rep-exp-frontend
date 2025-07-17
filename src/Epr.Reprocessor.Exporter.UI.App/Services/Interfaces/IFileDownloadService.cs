using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IFileDownloadService
{
    Task<byte[]?> GetFileAsync(
        Guid fileId,
        string fileName,
        SubmissionType submissionType,
        Guid submissionId);
}
