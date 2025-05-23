using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

public enum TaskStatuses
{
    NotStarted = 1,
    Started = 2,
    CannotStartYet = 3,
    Queried = 4,   
    Completed = 5
}
