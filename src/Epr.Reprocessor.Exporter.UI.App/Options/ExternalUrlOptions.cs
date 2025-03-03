using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options
{
    [ExcludeFromCodeCoverage]
    public class ExternalUrlOptions
    {
        public const string ConfigSection = "ExternalUrls";

        public string LandingPage { get; set; }
        public string GovUkHome { get; set; }
        public string PrivacyPage { get; set; }
        public string AccessibilityPage { get; set; }

    }
}
