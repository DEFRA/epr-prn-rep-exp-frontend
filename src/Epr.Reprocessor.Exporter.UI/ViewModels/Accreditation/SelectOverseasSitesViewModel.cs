using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class SelectOverseasSitesViewModel : AccreditationBaseViewModel
    {
        public string SiteAddress { get; set; } = string.Empty;

        public List<SelectListItem> OverseasSites { get; set; } = new();
        [Required(ErrorMessage = "Select at least one overseas site.")]
        public List<string> SelectedOverseasSites { get; set; } = new();

        public int SelectedOverseasSitesCount => SelectedOverseasSites?.Count ?? 0;       
    }
}