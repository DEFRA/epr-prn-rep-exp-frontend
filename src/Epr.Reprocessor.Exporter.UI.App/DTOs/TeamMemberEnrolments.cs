namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class TeamMemberEnrolments
{
    public int EnrolmentId { get; set; }

    public Guid PersonId { get; set; }

    public Guid OrganisationId { get; set; }

    public int ServiceRoleId { get; set; }

    public int EnrolmentStatusId { get; set; }

    public string EnrolmentStatusName { get; set; }

    public string ServiceRoleKey { get; set; }

    public string AddedBy { get; set; }

    public string ViewDetails { get; set; }
}