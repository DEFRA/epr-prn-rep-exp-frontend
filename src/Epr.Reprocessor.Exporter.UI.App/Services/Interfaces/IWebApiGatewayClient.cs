using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IWebApiGatewayClient
{
    Task<Guid> UploadFileAccreditationAsync(
        byte[] byteArray,
        string fileName,        
        SubmissionType submissionType,
        Guid? submissionId);

    public Task<T?> GetSubmissionAsync<T>(Guid id)
        where T : AbstractSubmission;
}
