using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class CarrierBrokerDealerViewModel: IValidatableObject
{
    [RegularExpression(ValidationRegExConstants.ReferenceNumber, ErrorMessageResourceType = typeof(CarrierBrokerDealer), ErrorMessageResourceName = "enter_registration_number_error")]
    [MaxNumberValidation(MaxCharacters = 16, ErrorMessageResourceType = typeof(CarrierBrokerDealer), ErrorMessageResourceName = "enter_registration_number_error_max_characters")]
    public string? RegistrationNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? NationCode { get; set; }
    public bool? HasRegistrationNumber { get; set; }
    public bool RegisteredWasteCarrierBrokerDealerFlag { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
    
        if (NationCode == NationCodes.NorthernIreland)
        {
            if(HasRegistrationNumber is null )
            {
                yield return new ValidationResult(CarrierBrokerDealer.select_registration_error, new List<string> { nameof(HasRegistrationNumber) });
            }

            if (string.IsNullOrEmpty(RegistrationNumber) && HasRegistrationNumber.GetValueOrDefault() == true)
            {
                yield return new ValidationResult(CarrierBrokerDealer.enter_registration_number_blank_error, new List<string> { nameof(RegistrationNumber) });
            }
        }
        else
        {
            if (string.IsNullOrEmpty(RegistrationNumber))
            {
                yield return new ValidationResult(CarrierBrokerDealer.enter_registration_number_blank_error, new List<string> { nameof(RegistrationNumber) });
            }
        }
    }
}
