using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;

[ExcludeFromCodeCoverage]
public class OverseasAccreditationSiteDto
{
    public Guid ExternalId { get; set; }

    public string OrganisationName { get; set; }

    public int MeetConditionsOfExportId { get; set; }

    public int SiteCheckStatusId { get; set; }
}
