using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    [ExcludeFromCodeCoverage]
    public class CarrierBrokerDealerViewModel
    {
        [Required(ErrorMessageResourceName = "enter_registration_number_error", ErrorMessageResourceType = typeof(CarrierBrokerDealer))]
        [RegularExpression(ValidationRegExConstants.StringLength16Characters, ErrorMessageResourceType = typeof(CarrierBrokerDealer), ErrorMessageResourceName = "enter_registration_number_error_max_characters")]
        public string? RegistrationNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? NationCode { get; set; }
        [Required(ErrorMessageResourceName = "select_registration_error", ErrorMessageResourceType = typeof(CarrierBrokerDealer))]
        public bool? HasRegistrationNumber { get; set; }
    }
}
