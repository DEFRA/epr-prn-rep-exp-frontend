using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class SelectAuthorityViewModel: AccreditationBaseViewModel
    {
        public string SiteAddress { get; set; } = string.Empty;

        public List<SelectListItem> Authorities { get; set; } = [];
        public List<string> SelectedAuthorities { get; set; } = [];
        public int SelectedAuthoritiesCount => SelectedAuthorities.Count;

        public string HomePageUrl { get; set; }
    }
}
