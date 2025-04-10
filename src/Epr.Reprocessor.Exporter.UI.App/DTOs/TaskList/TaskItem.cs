using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
public class TaskItem
{
    public string TaskName { get; set; }
    public string TaskLink { get; set; }
    public TaskListStatus status { get; set; }
}

