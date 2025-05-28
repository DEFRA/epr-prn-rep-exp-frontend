using Epr.Reprocessor.Exporter.UI.App.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class TaskListViewModel
    {
        public string Subject { get; set; } = "PRN";

        public Guid AccreditationId { get; set; }

        public bool IsApprovedUser { get; set; }

        public TaskListStatus TonnageAndAuthorityToIssuePrnStatus { get; set; }

        public TaskListStatus BusinessPlanStatus { get; set; }

        public TaskListStatus AccreditationSamplingAndInspectionPlanStatus { get; set; }

        public PeopleAbleToSubmitApplicationViewModel PeopleCanSubmitApplication { get; set; }

        public string PrnTonnageRouteName { get; set; }

        public string SamplingInspectionRouteName { get; set; }

        public bool AllTasksCompleted => TonnageAndAuthorityToIssuePrnStatus == TaskListStatus.Completed &&
                                         BusinessPlanStatus == TaskListStatus.Completed &&
                                         AccreditationSamplingAndInspectionPlanStatus == TaskListStatus.Completed;
    }
}
