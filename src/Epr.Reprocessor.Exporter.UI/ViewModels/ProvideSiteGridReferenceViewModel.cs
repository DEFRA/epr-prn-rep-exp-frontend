using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ProvideSiteGridReferenceViewModel
    {
        [Required(ErrorMessageResourceName = "enter_site_grid_reference_error_message", ErrorMessageResourceType =typeof(ProvideSiteGridReference))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "grid_reference_must_include_numbers_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [MinLength(4, ErrorMessageResourceName = "grid_reference_with_at_least_4_characters_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [MaxNumberValidation(ErrorMessageResourceName = "grid_reference_with_no_more_than_10_characters_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [Display(ResourceType = typeof(ProvideSiteGridReference), Name = "enter_site_grid_reference")]
        public string GridReference { get; set; }
        public string Address { get; set; } = "1, Rhyl Coast Road, Rhyl, Denbighshire";
    }
}
