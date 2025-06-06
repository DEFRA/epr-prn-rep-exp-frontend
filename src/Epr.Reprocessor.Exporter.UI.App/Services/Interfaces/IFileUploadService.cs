using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<Guid> UploadFileAccreditationAsync(
            byte[] byteArray,
            string fileName,
            SubmissionType submissionType,
            Guid? submissionId = null);

        Task<T> GetFileUploadStatusAsync<T>(Guid submissionId) where T : AbstractSubmission;
    }
}
