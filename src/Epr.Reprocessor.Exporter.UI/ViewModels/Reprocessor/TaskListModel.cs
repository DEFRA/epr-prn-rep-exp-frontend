namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

/// <summary>
/// Represents the model for the task list page.
/// </summary>
public class TaskListModel
{
    /// <summary>
    /// Collection of tasks to be completed.
    /// </summary>
    public IList<TaskItem> TaskList { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Have all tasks been completed.
    /// </summary>
    public bool HaveAllBeenCompleted => TaskList.All(task => task.Status.Equals(ApplicantRegistrationTaskStatus.Completed));

    public ApplicantRegistrationTaskStatus WasteLicenseStatus
    {
        get

        {
            var tskSite = TaskList.SingleOrDefault(o => o.TaskName == TaskType.SiteAddressAndContactDetails);
            var tskWaste = TaskList.SingleOrDefault(o => o.TaskName == TaskType.WasteLicensesPermitsAndExemptions);

            if (tskSite == null || tskWaste == null)
            {
                return ApplicantRegistrationTaskStatus.CannotStartYet;
            }

            if (tskSite.Status.Equals(ApplicantRegistrationTaskStatus.Completed) && tskWaste.Status.Equals(ApplicantRegistrationTaskStatus.CannotStartYet))
            {
                return ApplicantRegistrationTaskStatus.NotStarted;
            }

            return ApplicantRegistrationTaskStatus.CannotStartYet;
        }
    }
}