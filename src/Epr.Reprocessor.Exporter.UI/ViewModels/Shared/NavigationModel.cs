using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared
{
    [ExcludeFromCodeCoverage]
    public class NavigationModel
    {
        public string LocalizerKey { get; set; }

        public string LinkValue { get; set; }

        public bool IsActive { get; set; }
    }
}
