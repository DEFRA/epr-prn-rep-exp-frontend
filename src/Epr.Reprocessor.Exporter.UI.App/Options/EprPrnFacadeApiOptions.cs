
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options
{
    [ExcludeFromCodeCoverage]
    public class EprPrnFacadeApiOptions
    {
        public const string ConfigSection = "EprPrnFacadeAPI";

        public string BaseEndpoint { get; set; }

        public string DownstreamScope { get; set; }
    }
}
