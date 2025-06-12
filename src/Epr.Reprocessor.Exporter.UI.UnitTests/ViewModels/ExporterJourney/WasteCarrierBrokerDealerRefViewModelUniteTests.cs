using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.ExporterJourney
{
    [TestClass]
    public class WasteCarrierBrokerDealerRefViewModelUnitTests
    {
        [TestMethod]
        public void WasteCarrierBrokerDealerRefViewModel_ValidModel_AllFieldsValid()
        {
            // Arrange
            var model = new WasteCarrierBrokerDealerRefViewModel
            {
                WasteCarrierBrokerDealerRegistrationNumber = "CBDU123456"
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        public void WasteCarrierBrokerDealerRefViewModel_InvalidModel_RegistrationNumberTooLong()
        {
            // Arrange
            var model = new WasteCarrierBrokerDealerRefViewModel
            {
                WasteCarrierBrokerDealerRegistrationNumber = new string('A', 51) // Assuming MaxLength(50)
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Exists(v => v.MemberNames.Contains(nameof(model.WasteCarrierBrokerDealerRegistrationNumber))));
        }

        [TestMethod]
        public void WasteCarrierBrokerDealerRefViewModel_ValidModel_EmptyRegistrationNumber()
        {
            // Arrange
            var model = new WasteCarrierBrokerDealerRefViewModel
            {
                WasteCarrierBrokerDealerRegistrationNumber = string.Empty
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
    }
}