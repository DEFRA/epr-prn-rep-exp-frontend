using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class PrnTonnageViewModel
    {
        public string MaterialName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "error_message", ErrorMessageResourceType = typeof(ViewResources.PrnTonnage))]
        public string? PrnTonnage { get; set; }
    }
}
