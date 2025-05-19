using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class SelectAuthorityViewModel
    {
        public Guid AccreditationId { get; set; }
        public string SiteAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = "PRN";
        public string? Action { get; set; }

        public List<SelectListItem> Authorities { get; set; } = [];
        public List<string> SelectedAuthorities { get; set; } = [];


  
        public int SelectedAuthoritiesCount => SelectedAuthorities.Count;
    }
}
