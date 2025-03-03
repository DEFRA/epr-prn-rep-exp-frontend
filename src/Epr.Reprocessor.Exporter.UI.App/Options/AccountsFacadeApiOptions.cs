using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class AccountsFacadeApiOptions
{
    public const string ConfigSection = "AccountsFacadeAPI";

    public string BaseEndpoint { get; set; }

    public string DownstreamScope { get; set; }
}
