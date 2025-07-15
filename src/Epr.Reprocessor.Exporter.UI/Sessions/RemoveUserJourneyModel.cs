namespace Epr.Reprocessor.Exporter.UI.Sessions;

public class RemoveUserJourneyModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid PersonId { get; set; }
    public Guid OrganisationId { get; set; }
    public string Role { get; set; }
    public int EnrolmentId { get; set; }
    public bool IsRemoved { get; set; }
}