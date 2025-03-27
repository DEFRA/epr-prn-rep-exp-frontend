using System.ComponentModel;
using System.Drawing.Text;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

public class TaskListModel
{
    public IList<TaskItem> TaskList { get; set; }
    public bool TaskListComplete
    {
        get
        {
            return TaskList != null && TaskList.All(task => task.status == TaskListStatus.COMPLETED);
        }
    }
}

public class TaskItem
{
    public string TaskName { get; set; } 
    public string TaskLink { get; set; }
    public TaskListStatus status { get; set; }
}

public enum TaskListStatus
{
    [Description("CANNOT START YET")]
    CannotStartYet = 1,
    [Description("NOT STARTED")]
    NotStart = 2,
    [Description("IN PROGRESS")]
    InProgress = 3,
    [Description("COMPLETED")]
    COMPLETED = 4,

} 