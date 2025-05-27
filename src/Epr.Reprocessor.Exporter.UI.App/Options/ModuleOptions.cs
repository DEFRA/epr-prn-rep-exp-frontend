using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options
{
    [ExcludeFromCodeCoverage]
    public class ModuleOptions
    {
        public const string ConfigSection = "Module";
        public string ServiceKey { get; set; }
    }
}
