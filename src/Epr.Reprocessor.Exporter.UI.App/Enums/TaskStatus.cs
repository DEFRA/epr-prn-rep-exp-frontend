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
public enum ReprocessorExporterTaskStatus
{
    None = 0,
    NotStarted = 1,
    Started = 2,
    CannotStartYet = 3,
    Queried = 4,
    Completed = 5,
    Unknown = 99
}