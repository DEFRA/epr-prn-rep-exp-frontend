namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class SelectAuthorisationTypeViewModelUnitTests
{
    [TestMethod]
    public void SelectAuthorisationTypeViewModel_ValidModel_WasteExemption_NoPermitNumberNecessary_ButProvidePermitToEnsureSuccessfulValidation()
    {
        // Arrange
        var model = new SelectAuthorisationTypeViewModel
        {
            SelectedAuthorisation = 1,
            AuthorisationTypes =
            [
                new()
                {
                    Id = 1,
                    Label = "label",
                    Name = "ppc",
                    // Setting an in correct permit number, but it shouldn't matter as it's a waste exemption permit type we don't need a permit number.
                    SelectedAuthorisationText = "EPR/"
                }
            ]
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void SelectAuthorisationTypeViewModel_ValidModel_SelectedAuthorisationPopulatedAndTextPopulatedWithCorrectFormat()
    {
        // Arrange
        var model = new SelectAuthorisationTypeViewModel
        {
            SelectedAuthorisation = 2,
            AuthorisationTypes =
            [
                new()
                {
                    Id = 2,
                    Label = "label",
                    Name = "ppc",
                    SelectedAuthorisationText = "EPR/AB1234CD"
                }
            ]
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void SelectAuthorisationTypeViewModel_InvalidModel_WrongPermitNumberFormat()
    {
        // Arrange
        var model = new SelectAuthorisationTypeViewModel
        {
            SelectedAuthorisation = 3,
            AuthorisationTypes =
            [
                new()
                {
                    Id = 3,
                    Label = "label",
                    Name = "ppc",
                    SelectedAuthorisationText = "EPR/"
                }
            ]
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().HaveCount(1);
    }
}