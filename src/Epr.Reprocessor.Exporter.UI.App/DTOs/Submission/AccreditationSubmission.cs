using Epr.Reprocessor.Exporter.UI.App.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;

[ExcludeFromCodeCoverage]
public class AccreditationSubmission : AbstractSubmission
{
    public override SubmissionType Type => SubmissionType.Accreditation;

    public Guid FileId { get; set; }

    public string AccreditationFileName { get; set; }

    public DateTime? AccreditationFileUploadDateTime { get; set; }

    public bool AccreditationDataComplete { get; set; }
}
