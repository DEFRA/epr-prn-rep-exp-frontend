using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount
{
    [ExcludeFromCodeCoverage]
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public string EnrolmentStatus { get; set; }
        
        //Roles with is service like Approved, Delegated
        public string ServiceRole { get; set; }

        //Service this enrolement is for eg: Packaging, reprocessor etc
        public string Service { get; set; }
        public int ServiceRoleId { get; set; }
        public string? ServiceRoleKey { get; set; }
    }
}