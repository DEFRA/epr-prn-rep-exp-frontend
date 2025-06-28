namespace Epr.Reprocessor.Exporter.UI.ViewModels.Team;

[ExcludeFromCodeCoverage]
public class TeamViewModel
{
    public string? OrganisationName { get; set; }
    public string? OrganisationNumber { get; set; }
    public string AddNewUser { get; set; }
    public string AboutRolesAndPermissions { get; set; }
    public List<string> UserServiceRoles { get; set; }
    public Guid? ExternalId { get; set; }
    public List<TeamMemberViewModel> TeamMembers { get; set; }
}