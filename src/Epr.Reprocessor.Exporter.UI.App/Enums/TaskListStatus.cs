using System.ComponentModel;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

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
