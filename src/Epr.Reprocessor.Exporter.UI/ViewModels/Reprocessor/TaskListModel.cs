using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.Threading.Tasks;

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
    public bool HaveAllBeenCompleted => TaskList.All(task => task.TaskStatus.Equals(TaskStatus.Completed));

    public TaskStatus WasteLicenseStatus
    {
        get

        {
            var tskSite = TaskList.SingleOrDefault(o => o.TaskType == TaskType.SiteAndContactDetails);
            var tskWaste = TaskList.SingleOrDefault(o => o.TaskType == TaskType.WasteLicensesPermitsExemptions);

            if (tskSite == null || tskWaste == null)
            {
                return TaskStatus.CannotStartYet;
            }

            if (tskSite.TaskStatus.Equals(TaskStatus.Completed) && tskWaste.TaskStatus.Equals(TaskStatus.CannotStartYet))
            {
                return TaskStatus.NotStart;
            }

            return TaskStatus.CannotStartYet;
        }
    }
}