using Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class InterimSitesQuestionOneViewModelValidatorTests
{
    private InterimSitesQuestionOneViewModelValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new InterimSitesQuestionOneViewModelValidator();
    }

    [TestMethod]
    public void HasInterimSites_True_Should_Be_Valid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };

        //Act
        var result = _validator.Validate(model);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void HasInterimSites_False_Should_Be_Valid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };

        //Act
        var result = _validator.Validate(model);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void HasInterimSites_Null_Should_Be_Invalid()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = null };
        //Act
        var result = _validator.Validate(model);

        //Assert
        result.IsValid.Should().BeFalse();
    }
}
