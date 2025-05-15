
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount
{
    [ExcludeFromCodeCoverage]
    public class ManageUserDto
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
    }
}