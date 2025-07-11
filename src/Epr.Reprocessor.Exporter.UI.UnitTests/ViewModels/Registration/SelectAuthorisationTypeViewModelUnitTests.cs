namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class SelectAuthorisationTypeViewModelUnitTests
{
    [TestMethod]
    public void SelectAuthorisationTypeViewModel_InvalidModel_RequiredFieldNotProvided()
    {
        // Arrange
        var model = new SelectAuthorisationTypeViewModel
        {
            SelectedAuthorisation = null
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new("Select the type of permit the site has for accepting and recycling this waste", new List<string>{"SelectedAuthorisation"})
        });
    }

    [TestMethod]
    public void SelectAuthorisationTypeViewModel_ValidModel_RequiredFieldProvided()
    {
        // Arrange
        var model = new SelectAuthorisationTypeViewModel
        {
            SelectedAuthorisation = 1
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}