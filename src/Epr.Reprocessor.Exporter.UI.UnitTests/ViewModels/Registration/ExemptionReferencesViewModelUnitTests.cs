namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class ExemptionReferencesViewModelUnitTests
{
    [TestMethod]
    public void ExemptionReferencesViewModel_ValidModel_OneFieldPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "value"
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_ValidModel_AllFieldsPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "value1",
            ExemptionReferences2 = "value2",
            ExemptionReferences3 = "value3",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_NoFieldsPopulated_EnsureValidationResultsPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel();

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult> { new("Enter at least one exemption reference", new List<string>{nameof(model.ExemptionReferences1)}) };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_OneDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "value3",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_TwoDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "duplicate",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2), nameof(model.ExemptionReferences3) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_TwoDistinctDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "duplicate1",
            ExemptionReferences4 = "duplicate1",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2) }),
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences4) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    [DataRow("abc def")]         // contains space
    [DataRow("123 / 456")]       // contains spaces around slash
    [DataRow("hello.world")]     // contains period
    [DataRow("foo*bar")]         // contains asterisk
    [DataRow("user$name")]       // contains dollar sign
    [DataRow("path\\file")]      // contains backslash
    [DataRow("abc@domain.com")]  // contains @ and dot
    [DataRow("naïve")]           // contains accent
    [DataRow("東京")]             // contains Unicode characters
    [DataRow("123.45")]          // contains dot

    public void ExemptionReferencesViewModel_ValidateRegex_Invalid(string input)
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = input,
            ExemptionReferences2 = input,
            ExemptionReferences3 = input,
            ExemptionReferences4 = input,
            ExemptionReferences5 = input
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().HaveCount(5);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_ValidateStringLength_Invalid()
    {
        // Arrange
        var invalidLength = new string('a', 21);
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = invalidLength,
            ExemptionReferences2 = invalidLength,
            ExemptionReferences3 = invalidLength,
            ExemptionReferences4 = invalidLength,
            ExemptionReferences5 = invalidLength
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().HaveCount(5);
    }
}