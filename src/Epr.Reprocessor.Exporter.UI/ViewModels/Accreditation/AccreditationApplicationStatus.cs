using Epr.Reprocessor.Exporter.UI.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class AccreditationApplicationStatus
    {
        public string ReprocessorAddress { get; set; } = string.Empty;

        public Dictionary<string, RegistrationStatus> Status { get; set; } = new Dictionary<string, RegistrationStatus>();

    }
}
