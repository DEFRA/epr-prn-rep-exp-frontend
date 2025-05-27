using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount
{
    [ExcludeFromCodeCoverage]
    public class Enrolment
    {
        public int enrolmentId { get; set; }
        public string enrolmentStatus { get; set; }
        public string serviceRole { get; set; }
        public string service { get; set; }
        public int serviceRoleId { get; set; }
    }
}