using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class SessionOptions
{
    public const string ConfigSection = "Session";

    public int IdleTimeoutMinutes { get; set; }
}
