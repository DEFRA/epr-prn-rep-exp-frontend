using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

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