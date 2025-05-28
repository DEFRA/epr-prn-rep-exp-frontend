using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class PostcodeLookupFacadeApiOptions
{
    public const string ConfigSection = "PostcodeLookupFacadeAPI";

    public string BaseEndpoint { get; set; }

    public string DownstreamScope { get; set; }
}
