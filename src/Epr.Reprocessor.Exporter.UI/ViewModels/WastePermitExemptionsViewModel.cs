using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class WastePermitExemptionsViewModel
    {
        public List<SelectListItem> Materials { get; set; } = [];
        [Required(ErrorMessage = "Select all the material categories the site has a permit or exemption to accept and recycle")]
        public List<string> SelectedMaterials { get; set; } = [];
    }

