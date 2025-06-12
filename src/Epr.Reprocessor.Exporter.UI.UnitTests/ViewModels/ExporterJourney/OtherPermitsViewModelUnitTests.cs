using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.ExporterJourney
{
    [TestClass]
    public class OtherPermitsViewModelUnitTests
    {
        [TestMethod]
        public void OtherPermitsViewModel_ValidModel_AllFieldsValid()
        {
            // Arrange
            var model = new OtherPermitsViewModel
            {
                Id = 1,
                RegistrationId = 2,
                WasteLicenseOrPermitNumber = "ABC123",
                PpcNumber = "PPC456",
                WasteExemptionReference = new List<string> { "EX1", "EX2" }
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        public void OtherPermitsViewModel_InvalidModel_WasteLicenseOrPermitNumberTooLong()
        {
            // Arrange
            var model = new OtherPermitsViewModel
            {
                WasteLicenseOrPermitNumber = new string('A', 51)
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Exists(v => v.MemberNames.Contains(nameof(model.WasteLicenseOrPermitNumber))));
        }

        [TestMethod]
        public void OtherPermitsViewModel_InvalidModel_PpcNumberTooLong()
        {
            // Arrange
            var model = new OtherPermitsViewModel
            {
                PpcNumber = new string('B', 51)
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Exists(v => v.MemberNames.Contains(nameof(model.PpcNumber))));
        }

        [TestMethod]
        public void OtherPermitsViewModel_ValidModel_WasteExemptionReferenceList()
        {
            // Arrange
            var model = new OtherPermitsViewModel
            {
                WasteExemptionReference = new List<string> { "EX1", "EX2" }
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