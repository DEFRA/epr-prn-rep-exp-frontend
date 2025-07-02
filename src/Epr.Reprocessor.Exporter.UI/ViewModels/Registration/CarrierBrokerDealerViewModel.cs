using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class CarrierBrokerDealerViewModel : IValidatableObject
{
    #region private properties
    private const int MAX_CHARCTERS = 16;
    private const int MAX_NUMBERS = 10;
    private const int MAX_LETTERS = 10;
    private const int MAX_CHARCTERS_NIR = 11;
    private const int MAX_NUMBERS_NIR = 6;
    private const int MAX_LETTERS_NIR = 6;
    #endregion

    /// <summary>
    /// The registration number
    /// </summary>
    [RegularExpression(ValidationRegExConstants.ReferenceNumber, ErrorMessageResourceType = typeof(CarrierBrokerDealer), ErrorMessageResourceName = "enter_registration_number_error")]
    public string? RegistrationNumber { get; set; }
    /// <summary>
    /// The company name
    /// </summary>
    public string? CompanyName { get; set; }
    /// <summary>
    /// The company's nation
    /// </summary>
    public string? NationCode { get; set; }
    /// <summary>
    /// A boolean value on whether the company has a registration
    /// </summary>
    public bool? HasRegistrationNumber { get; set; }
    /// <summary>
    /// The registration flag
    /// </summary>
    public bool RegisteredWasteCarrierBrokerDealerFlag { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NationCode == UkNation.NorthernIreland.ToString())
        {
            if (HasRegistrationNumber is null)
            {
                yield return new ValidationResult(CarrierBrokerDealer.select_registration_error, new List<string> { nameof(HasRegistrationNumber) });
                yield break;
            }

            if (string.IsNullOrEmpty(RegistrationNumber) && HasRegistrationNumber.GetValueOrDefault())
            {
                yield return new ValidationResult(CarrierBrokerDealer.enter_registration_number_blank_error, new List<string> { nameof(RegistrationNumber) });
                yield break;
            }
            else
            {
                var validationResultRegisrationNumber = ValidateRegistrationNumber();

                if (validationResultRegisrationNumber is not null)
                {
                    yield return validationResultRegisrationNumber;
                    yield break;
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(RegistrationNumber))
            {
                yield return new ValidationResult(CarrierBrokerDealer.enter_registration_number_blank_error, new List<string> { nameof(RegistrationNumber) });
                yield break;
            }
            else
            {
                var validationResultRegisrationNumber = ValidateRegistrationNumber();

                if(validationResultRegisrationNumber is not null)
                {
                    yield return validationResultRegisrationNumber;
                    yield break;
                }
            }
        }   
     }

    private ValidationResult? ValidateRegistrationNumber()
    {
        var registrationLength = RegistrationNumber?.Length;
        var registrationLetterTextLength = new String(RegistrationNumber.Where(Char.IsLetter).ToArray())?.Length;
        var registrationNumberTextLength = GetNumbersFromStringRegEx.GetValue(RegistrationNumber)?.Length;
        var isNorthernIreland = NationCode == UkNation.NorthernIreland.ToString();

        if (registrationLength > (isNorthernIreland ? MAX_CHARCTERS_NIR : MAX_CHARCTERS))
        {
            return new ValidationResult(isNorthernIreland ? CarrierBrokerDealer.nothernireland_enter_registration_number_error_max_characters :  CarrierBrokerDealer.enter_registration_number_error_max_characters, new List<string> { nameof(RegistrationNumber) });
        }

        if (registrationNumberTextLength > (isNorthernIreland ? MAX_NUMBERS_NIR :MAX_NUMBERS))
        {
            return new ValidationResult(isNorthernIreland ? CarrierBrokerDealer.nothernireland_enter_registration_number_error_max_numbers :  CarrierBrokerDealer.enter_registration_number_error_max_numbers, new List<string> { nameof(RegistrationNumber) });

        }

        if (registrationLetterTextLength > (isNorthernIreland ? MAX_LETTERS_NIR: MAX_LETTERS))
        {
            return new ValidationResult(isNorthernIreland ? CarrierBrokerDealer.nothernireland_enter_registration_number_error_max_letters : CarrierBrokerDealer.enter_registration_number_error_max_letters, new List<string> { nameof(RegistrationNumber) });
        }

        return null;
    }
 }

