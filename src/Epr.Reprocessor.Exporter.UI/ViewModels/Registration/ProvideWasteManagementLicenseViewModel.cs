using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    public class ProvideWasteManagementLicenseViewModel
    {
        [Required(ErrorMessageResourceName = "error_message_select_weight", ErrorMessageResourceType = typeof(ProvideWasteManagementLicense))]
        [RegularExpression(ValidationRegExConstants.GreaterThen0, ErrorMessageResourceName = "error_message_weight_must_be_more_than_0", ErrorMessageResourceType = typeof(ProvideWasteManagementLicense))]
        [Range(0, 10000000, ErrorMessageResourceName = "error_message_weight_must_be_less_than_10000000", ErrorMessageResourceType = typeof(ProvideWasteManagementLicense))]
        [IsNumeric(ErrorMessageResourceName = "error_message_weight_must_be_number", ErrorMessageResourceType = typeof(ProvideWasteManagementLicense))]
        public string Weight { get; set; }
        [Required(ErrorMessageResourceName = "error_message_select_authorised_weight_per_year_month_week", ErrorMessageResourceType = typeof(ProvideWasteManagementLicense))]
        public string SelectedFrequency { get; set; }
    }
}
