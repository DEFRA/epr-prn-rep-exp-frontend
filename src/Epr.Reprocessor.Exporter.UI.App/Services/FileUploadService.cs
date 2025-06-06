using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services
{
    public class FileUploadService(IWebApiGatewayClient webApiGatewayClient) : IFileUploadService
    {
        public async Task<Guid> UploadFileAccreditationAsync(
            byte[] byteArray,
            string fileName,
            SubmissionType submissionType,
            Guid? submissionId = null)
        {
            return await webApiGatewayClient.UploadFileAccreditationAsync(byteArray, fileName, submissionType, submissionId);
        }

        public async Task<T> GetFileUploadStatusAsync<T>(Guid submissionId) where T : AbstractSubmission
        {
            return await webApiGatewayClient.GetSubmissionAsync<T>(submissionId);
        }
    }
}
