﻿using System.ComponentModel;

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
