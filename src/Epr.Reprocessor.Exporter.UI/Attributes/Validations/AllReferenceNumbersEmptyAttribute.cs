using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

namespace Epr.Reprocessor.Exporter.UI.Attributes.Validations
{
    public class AllReferenceNumbersEmptyAttribute: ValidationAttribute
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

            // Check if all reference numbers are empty
            if (string.IsNullOrEmpty(viewModel.ExemptionReferences1) ||
                string.IsNullOrEmpty(viewModel.ExemptionReferences2) ||
                string.IsNullOrEmpty(viewModel.ExemptionReferences3) ||
                string.IsNullOrEmpty(viewModel.ExemptionReferences4) ||
                string.IsNullOrEmpty(viewModel.ExemptionReferences5))
            {
                return new ValidationResult(ErrorMessage ?? ExemptionReferences.ErrorMessageBlank);
            }

            return ValidationResult.Success;
        }
    }
}
