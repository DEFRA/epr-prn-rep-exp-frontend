using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class ProvideGridReferenceOfReprocessingSiteViewModel
    {
        [Required(ErrorMessageResourceName = "enter_site_grid_reference_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [RegularExpression(ValidationRegExConstants.GridReferenceNoSpecialCharacters, ErrorMessageResourceName = "grid_reference_must_include_numbers_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [MatchAtLeastValidation(ErrorMessageResourceName = "grid_reference_with_at_least_4_characters_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [MaxNumberValidation(ErrorMessageResourceName = "grid_reference_with_no_more_than_10_characters_error_message", ErrorMessageResourceType = typeof(ProvideSiteGridReference))]
        [Display(ResourceType = typeof(ProvideSiteGridReference), Name = "enter_site_grid_reference")]
        public string GridReference { get; set; }
    }
}
