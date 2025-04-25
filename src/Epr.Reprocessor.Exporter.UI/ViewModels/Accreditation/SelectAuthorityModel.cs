using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class SelectAuthorityModel
    {
        public List<SelectListItem> Authorities { get; set; }
        public List<string> SelectedAuthorities { get; set; }



        public SelectAuthorityModel()
        {
            Authorities = new List<SelectListItem>();


           
            SelectedAuthorities = new List<string>(); 
        }

        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "error_message", ErrorMessageResourceType = typeof(ViewResources.SelectAuthority)) ]

    
        public int SelectedAuthoritiesCount => SelectedAuthorities.Count;
    }
}
