namespace Epr.Reprocessor.Exporter.UI.ViewModels.Team;

public class TeamViewModel
{
    public string? OrganisationName { get; set; }
    public string? OrganisationNumber { get; set; }
    public string AddNewUser { get; set; }
    public string AboutRolesAndPermissions { get; set; }
    public string UserServiceRole { get; set; }
    public List<TeamMemberViewModel> TeamMembers { get; set; }
}