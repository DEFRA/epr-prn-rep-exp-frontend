using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class CheckAnswersViewModel
    {
        public string Subject { get; set; } = "PRN";

        public string? Action { get; set; }

        public Guid AccreditationId { get; set; }

        public int? PrnTonnage { get; set; }

        public string TonnageChangeRoutePath { get; set; }

        public string AuthorisedUsers { get; set; }

        public string AuthorisedUserChangeRoutePath { get; set; }
    }
}
