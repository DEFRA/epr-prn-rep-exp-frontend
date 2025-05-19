using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels
{
    [TestClass]
    public class UKSiteLocationViewModelTests
    {
        [TestMethod]
        public void SiteLocationId_ShouldBeRequired()
        {
            // Arrange
            var model = new UKSiteLocationViewModel();
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid); 
        }

        [TestMethod]
        public void SiteLocationId_ShouldBeValid_WhenSetToValidValue()
        {
            // Arrange
            var model = new UKSiteLocationViewModel { SiteLocationId = UkNation.England };
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
        }
    }
}
