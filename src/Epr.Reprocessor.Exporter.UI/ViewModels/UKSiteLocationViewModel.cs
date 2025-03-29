using Epr.Reprocessor.Exporter.UI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class UKSiteLocationViewModel
    {
        [Required]
        public UkNation? SiteLocationId { get; set; }
    }
}
