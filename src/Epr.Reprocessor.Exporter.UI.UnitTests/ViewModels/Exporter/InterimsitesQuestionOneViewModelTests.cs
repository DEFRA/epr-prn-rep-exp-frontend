using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Exporter;

[TestClass]
public class InterimsitesQuestionOneViewModelTests
{
    [TestMethod]
    public void HasInterimSites_True_Should_Be_Valid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, validationContext, results, true);

        // Assert
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void HasInterimSites_False_Should_Be_Valid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, validationContext, results, true);

        // Assert
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void HasInterimSites_Null_Should_Be_Invalid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = null };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, validationContext, results, true);

        // Assert
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("Please select an option", results[0].ErrorMessage);
    }
}

