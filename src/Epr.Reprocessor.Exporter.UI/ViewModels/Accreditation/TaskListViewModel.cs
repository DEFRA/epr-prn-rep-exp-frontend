using Epr.Reprocessor.Exporter.UI.App.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class TaskListViewModel
    {
        public string Subject { get; set; } = "PRN";

        public Guid AccreditationId { get; set; }

        public TaskListStatus TonnageAndAuthorityToIssuePrnStatus { get; set; }

        public bool IsApprovedUser { get; set; }

        public bool PrnTonnageChecked { get; set; }

        public bool BusinessPlanChecked { get; set; }

        public bool AccreditationSamplingAndInspectionPlanChecked { get; set; }

        public PeopleAbleToSubmitApplicationViewModel PeopleCanSubmitApplication { get; set; }
    }
}
