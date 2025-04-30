using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SelectMaterialViewModel
    {
        public List<SelectListItem> Materials { get; set; }

        [Required(ErrorMessageResourceName = "error_message", ErrorMessageResourceType = typeof(ViewResources.SelectMaterial))]
        public string? SelectedMaterial { get; set; }
    }
}
