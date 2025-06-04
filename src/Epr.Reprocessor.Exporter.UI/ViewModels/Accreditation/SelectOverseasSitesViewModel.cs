using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SelectOverseasSitesViewModel
    {
        public string Subject { get; set; } = "PERN";        
        public string? Action { get; set; }
        public Guid AccreditationId { get; set; }
        public List<SelectListItem> OverseasSites { get; set; } = new();        
        public List<string> SelectedOverseasSites { get; set; } = new();
        public int SelectedOverseasSitesCount => SelectedOverseasSites?.Count ?? 0;       
    }
}