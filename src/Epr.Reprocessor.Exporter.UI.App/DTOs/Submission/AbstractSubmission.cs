using Epr.Reprocessor.Exporter.UI.App.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;

[ExcludeFromCodeCoverage]
public abstract class AbstractSubmission
{
    public Guid Id { get; set; }

    public abstract SubmissionType Type { get; }

    public DateTime Created { get; set; }

    public bool ValidationPass { get; set; }

    public bool HasWarnings { get; set; }

    public bool HasValidFile { get; set; }

    public List<string> Errors { get; set; } = new();

    public bool IsSubmitted { get; set; }
}
