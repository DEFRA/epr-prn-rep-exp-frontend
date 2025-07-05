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
    public bool HaveAllBeenCompleted => TaskList.All(task => task.Status.Equals(TaskStatus.Completed));

    public TaskStatus WasteLicenseStatus
    {
        get

        {
            var tskSite = TaskList.SingleOrDefault(o => o.TaskName == TaskType.SiteAddressAndContactDetails);
            var tskWaste = TaskList.SingleOrDefault(o => o.TaskName == TaskType.WasteLicensesPermitsAndExemptions);

            if (tskSite == null || tskWaste == null)
            {
                return TaskStatus.CannotStartYet;
            }

            if (tskSite.Status.Equals(TaskStatus.Completed) && tskWaste.Status.Equals(TaskStatus.CannotStartYet))
            {
                return TaskStatus.NotStart;
            }

            return TaskStatus.CannotStartYet;
        }
    }
}