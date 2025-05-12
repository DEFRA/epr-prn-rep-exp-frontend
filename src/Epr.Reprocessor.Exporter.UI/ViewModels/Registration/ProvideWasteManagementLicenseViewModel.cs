using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    public class ProvideWasteManagementLicenseViewModel
    {
        [Required]
        public string Weight { get; set; }
        [Required]
        public string SelectedFrequency { get; set; }
    }
}
