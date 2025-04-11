using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums;
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



