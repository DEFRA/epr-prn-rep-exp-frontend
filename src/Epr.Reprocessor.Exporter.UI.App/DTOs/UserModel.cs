using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class UserModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid PersonId { get; set; }
    public int PersonRoleId { get; set; }
    public int ServiceRoleId { get; set; }
    public string ServiceRoleKey { get; set; }
    public EnrolmentStatus EnrolmentStatus { get; set; }
    public bool IsRemoveable { get; set; }
    public Guid ConnectionId { get; set; }
    public string AddedBy { get; set; }
}