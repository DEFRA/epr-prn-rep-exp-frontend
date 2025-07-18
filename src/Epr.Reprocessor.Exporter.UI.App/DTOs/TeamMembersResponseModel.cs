namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class TeamMembersResponseModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public Guid PersonId { get; set; }

    public Guid ConnectionId { get; set; }

    public IEnumerable<TeamMemberEnrolments> Enrolments { get; set; }

    public Uri ViewDetails { get; set; }
}
