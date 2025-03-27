using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    public class SelectAuthorityModel
    {
        public List<SelectListItem> Authorities { get; set; }
        public List<string> SelectedAuthorities { get; set; }

        public SelectAuthorityModel()
        {
            Authorities = new List<SelectListItem>
                {
                    new SelectListItem { Value = "myself", Text = "Myself", Group = new SelectListGroup { Name = "Myself@reprocessor.com" } },
                    new SelectListItem { Value = "andrew", Text = "Andrew Recycler", Group = new SelectListGroup { Name = "Andrew.Recycler@reprocessor.com" } },
                    new SelectListItem { Value = "gary1", Text = "Gary Package", Group = new SelectListGroup { Name = "Gary.Package1@reprocessor.com" } },
                    new SelectListItem { Value = "gary2", Text = "Gary Package", Group = new SelectListGroup { Name = "GaryWPackageP@reprocessor.com" } },
                    new SelectListItem { Value = "scott", Text = "Scott Reprocessor", Group = new SelectListGroup { Name = "Scott.Reprocessor@reprocessor.com" } }
                };

            // Pre-select some authorities
            SelectedAuthorities = new List<string> { "myself", "andrew" };
        }
    }
}
