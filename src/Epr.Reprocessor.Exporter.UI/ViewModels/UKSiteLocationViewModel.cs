using Epr.Reprocessor.Exporter.UI.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class UKSiteLocationViewModel : SaveAndContinueViewModel
    {
        [Required]
        public UkNation? SiteLocationId { get; set; }

        public UKSiteLocationViewModel()
        {
            Data = JsonConvert.SerializeObject(this);
        }
    }
}
