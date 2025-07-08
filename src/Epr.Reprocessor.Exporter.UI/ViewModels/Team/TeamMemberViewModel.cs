namespace Epr.Reprocessor.Exporter.UI.ViewModels.Team;

[ExcludeFromCodeCoverage]
public class TeamMemberViewModel
{
    public string PersonId { get; set; }

    public string FullName { get; set; }

    public List<string> RoleKey { get; set; }

    public string Permissions { get; set; }

    public string Email { get; set; }

    public string AddedBy { get; set; }

    public Uri ViewDetails { get; set; }
}