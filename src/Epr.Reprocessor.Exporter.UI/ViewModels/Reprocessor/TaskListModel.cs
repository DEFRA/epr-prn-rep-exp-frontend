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
    public bool HaveAllBeenCompleted => TaskList.All(task => task.Status == TaskStatus.Completed);
}