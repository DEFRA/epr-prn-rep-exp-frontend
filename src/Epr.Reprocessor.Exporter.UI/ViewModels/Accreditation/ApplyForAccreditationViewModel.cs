using Epr.Reprocessor.Exporter.UI.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class ApplyForAccreditationViewModel
    {
        public ApplyForAccreditationViewModel()
        {
            Statuses.Add(new AccreditationApplicationStatus
            {
                ReprocessorAddress = "23 Ruby street",
                Status = new Dictionary<string, RegistrationStatus>
                {
                     { "Steel", RegistrationStatus.Completed },
                     { "Plastic", RegistrationStatus.Submitted },
                     { "Wood", RegistrationStatus.Granted },

                }

            });

            Statuses.Add(new AccreditationApplicationStatus
            {
                ReprocessorAddress = "1 Diamond way",
                Status = new Dictionary<string, RegistrationStatus>
                {
                     { "Paper", RegistrationStatus.Granted },
                     { "Wood", RegistrationStatus.Granted },
                     { "Steel", RegistrationStatus.Queried },

                }
            });
        }

        public List<AccreditationApplicationStatus> Statuses { get; set; } = new List<AccreditationApplicationStatus>();


    }
}
