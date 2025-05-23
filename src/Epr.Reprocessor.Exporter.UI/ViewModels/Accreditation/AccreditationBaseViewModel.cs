using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class AccreditationBaseViewModel
    {
        public string? Action { get; set; }
        private AccreditationDto? accreditation { get; set; }
        public AccreditationDto? Accreditation
        {
            get => accreditation;
            set
            {
                accreditation = value;
                ApplicationType = GetApplicationType(accreditation.ApplicationTypeId);
            }
        }

        public List<AccreditationPrnIssueAuthDto> PrnIssueAuthorities { get; set; } = new();


        public ApplicationType ApplicationType { get; set; } = ApplicationType.Reprocessor;

        public ApplicationType GetApplicationType(int applicationTypeId)
        {
            if (!Enum.IsDefined(typeof(ApplicationType), applicationTypeId))
            {
                throw new InvalidOperationException($"The application type Id {applicationTypeId} is not valid.");
            }
            return (ApplicationType)applicationTypeId;
        }

        public string ApplicationTypeDescription { get => ApplicationType == ApplicationType.Reprocessor ? "PRN" : "PERN"; } // getApplicationTypeDescription(ApplicationType);}
        private string GetApplicationTypeDescription(ApplicationType applicationType)
        {
            return applicationType == ApplicationType.Reprocessor ? "PRN" : "PERN";
        }
    }
}
