using Epr.Reprocessor.Exporter.UI.App.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class TaskListViewModel : AccreditationBaseViewModel
    {
        public bool IsApprovedUser { get; set; }

        public TaskStatus TonnageAndAuthorityToIssuePrnStatus { get; set; }

        public TaskStatus BusinessPlanStatus { get; set; }

        public TaskStatus AccreditationSamplingAndInspectionPlanStatus { get; set; }

        public PeopleAbleToSubmitApplicationViewModel PeopleCanSubmitApplication { get; set; }

        public string PrnTonnageRouteName { get; set; }

        public string SamplingInspectionRouteName { get; set; }

        public string SelectOverseasSitesRouteName { get; set; }

        public bool AllTasksCompleted => TonnageAndAuthorityToIssuePrnStatus == TaskStatus.Completed &&
                                         BusinessPlanStatus == TaskStatus.Completed &&
                                         AccreditationSamplingAndInspectionPlanStatus == TaskStatus.Completed;
    }
}
