namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AccreditationPrnIssueAuthDto
{
    public Guid ExternalId { get; set; }
    public Guid AccreditationExternalId { get; set; }
    public int PersonId { get; set; }
}
