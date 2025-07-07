using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

public enum TaskStatus
{
    [Description("CANNOT START YET")]
    CannotStartYet = 1,
    [Description("NOT STARTED")]
    NotStart = 2,
    [Description("IN PROGRESS")]
    InProgress = 3,
    [Description("COMPLETED")]
    Completed = 4,

}

/// <summary>
/// Represents statuses for a task for reprocessor/exporter journey. Ideally we'd have only one, but we also have <see cref="TaskStatus"/>
/// so we need to consolidate at some point.
/// </summary>
public enum ApplicantRegistrationTaskStatus
{
    None = 0,

    [Display(ResourceType = typeof(Resources.Enums.TaskStatus), Name = "not_started")]
    NotStarted = 1,
    
    [Display(ResourceType = typeof(Resources.Enums.TaskStatus), Name = "started")]
    Started = 2,

    [Display(ResourceType = typeof(Resources.Enums.TaskStatus), Name = "cannot_start_yet")]
    CannotStartYet = 3,

    [Display(ResourceType = typeof(Resources.Enums.TaskStatus), Name = "queried")]
    Queried = 4,

    [Display(ResourceType = typeof(Resources.Enums.TaskStatus), Name = "completed")]
    Completed = 5,

    Unknown = 99
}