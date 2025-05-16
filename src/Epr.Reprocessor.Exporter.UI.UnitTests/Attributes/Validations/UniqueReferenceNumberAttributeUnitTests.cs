using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations
{
    [TestClass]
    public class UniqueReferenceNumberAttributeUnitTests
    {
        private static ValidationContext CreateValidationContext(ExemptionReferencesViewModel viewModel)
        {
            return new ValidationContext(viewModel);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenReferenceNumberIsNull()
        {
            var viewModel = new ExemptionReferencesViewModel();
            var attribute = new UniqueReferenceNumberAttribute();

            var result = attribute.GetValidationResult(null, CreateValidationContext(viewModel));

            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenReferenceNumberIsEmpty()
        {
            var viewModel = new ExemptionReferencesViewModel();
            var attribute = new UniqueReferenceNumberAttribute();

            var result = attribute.GetValidationResult(string.Empty, CreateValidationContext(viewModel));

            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenReferenceNumberIsUnique()
        {
            var viewModel = new ExemptionReferencesViewModel
            {
                ExemptionReferences1 = "REF1",
                ExemptionReferences2 = "REF2",
                ExemptionReferences3 = "REF3",
                ExemptionReferences4 = "REF4",
                ExemptionReferences5 = "REF5"
            };
            var attribute = new UniqueReferenceNumberAttribute();

            var result = attribute.GetValidationResult("REF6", CreateValidationContext(viewModel));

            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_ReturnsError_WhenReferenceNumberIsDuplicate()
        {
            var viewModel = new ExemptionReferencesViewModel
            {
                ExemptionReferences1 = "DUPLICATE",
                ExemptionReferences2 = "DUPLICATE",
                ExemptionReferences3 = "REF3",
                ExemptionReferences4 = "REF4",
                ExemptionReferences5 = "REF5"
            };
            var attribute = new UniqueReferenceNumberAttribute();

            var result = attribute.GetValidationResult("DUPLICATE", CreateValidationContext(viewModel));

           Assert.AreNotEqual(ValidationResult.Success, result);
           Assert.AreEqual("Exemption reference number already added\r\n", result.ErrorMessage);
        }
    }
}
