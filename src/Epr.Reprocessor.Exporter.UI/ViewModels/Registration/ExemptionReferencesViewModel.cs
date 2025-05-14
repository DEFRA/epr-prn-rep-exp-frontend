using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    /// <summary>
    /// The model that handles the data for the Exemption References View.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExemptionReferencesViewModel
    {
        public List<string> ExemptionReferences { get; set; } = new List<string>();
    }

}
