using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class UKSiteLocationViewModel
    {
        [Required(ErrorMessageResourceName = "select_the_country_the_reprocessing_site_is_located_in", ErrorMessageResourceType = typeof(UkSiteLocation))]
        public UkNation? SiteLocationId { get; set; }
    }
}
