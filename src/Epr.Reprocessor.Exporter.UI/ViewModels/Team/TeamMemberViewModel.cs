namespace Epr.Reprocessor.Exporter.UI.ViewModels.Team;

public class TeamMemberViewModel
{
    public string FullName { get; set; }
    public string Role { get; set; } // e.g. "Approved Person", "Basic User"
    public string Permissions { get; set; } 
}