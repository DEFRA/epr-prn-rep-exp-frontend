using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    public class UniqueReferenceNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checking if the model state is valid according to the below condition
        /// </summary>
        /// <param name="value">Value data being passed</param>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Success or Fail</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = (ExemptionReferencesViewModel)validationContext.ObjectInstance;
            var referenceNumber = (string)value;

            if (string.IsNullOrEmpty(referenceNumber))
            {
                // Reference number is not required, so no need to check for duplicates if it's empty
                return ValidationResult.Success;
            }

            // Checking if the entered reference number already exists in any of the other reference number properties
            var allReferenceNumbers = new List<string>
            {
                viewModel.ExemptionReferences1,
                viewModel.ExemptionReferences2,
                viewModel.ExemptionReferences3,
                viewModel.ExemptionReferences4,
                viewModel.ExemptionReferences5
            };

            // Removing the current reference number from the list before checking for duplicates
            allReferenceNumbers.Remove(referenceNumber);

            if (allReferenceNumbers.Any(rn => rn == referenceNumber))
            {
                return new ValidationResult(ErrorMessage ?? ExemptionReferences.ErrorMessageDuplicate);
            }

            return ValidationResult.Success;
        }
    }
}
