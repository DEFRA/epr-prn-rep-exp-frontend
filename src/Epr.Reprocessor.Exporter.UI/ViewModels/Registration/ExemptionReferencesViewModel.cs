using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    /// <summary>
    /// The model that handles the data for the Exemption References View.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExemptionReferencesViewModel : IValidatableObject
    {        
        [UniqueReferenceNumber(ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageDuplicate")]
        [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageInvalidFormat")]
        [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageTooLong")]
        public string? ExemptionReferences1 { get; set; }

        [UniqueReferenceNumber(ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageDuplicate")]
        [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageInvalidFormat")]
        [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageTooLong")]
        public string? ExemptionReferences2 { get; set; }

        [UniqueReferenceNumber(ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageDuplicate")]
        [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageInvalidFormat")]
        [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageTooLong")]
        public string? ExemptionReferences3 { get; set; }

        [UniqueReferenceNumber(ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageDuplicate")]
        [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageInvalidFormat")]
        [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageTooLong")]
        public string? ExemptionReferences4 { get; set; }
       
        [UniqueReferenceNumber(ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageDuplicate")]
        [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageInvalidFormat")]
        [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "ErrorMessageTooLong")]
        public string? ExemptionReferences5 { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ExemptionReferences1) &&
                string.IsNullOrWhiteSpace(ExemptionReferences2) && 
                string.IsNullOrWhiteSpace(ExemptionReferences3) && 
                string.IsNullOrWhiteSpace(ExemptionReferences4) && 
                string.IsNullOrWhiteSpace(ExemptionReferences5))
            {
                yield return new ValidationResult(ExemptionReferences.ErrorMessageBlank);
            }
        }
    }
}
              