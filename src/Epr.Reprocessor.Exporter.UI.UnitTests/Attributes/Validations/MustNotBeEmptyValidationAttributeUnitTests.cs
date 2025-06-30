using Epr.Reprocessor.Exporter.UI.Attributes.Validations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations;

[TestClass]
public class MustNotBeEmptyValidationAttributeUnitTests
{
    public class TestModel
    {
        [MustNotBeEmpty(ErrorMessage = "error")]
        public List<string> List { get; set; } = [];
    }

    [TestMethod]
    public void MustNotBeEmptyValidation_EmptyCollection_ReturnFalse()
    {
        // Arrange
        var sut = new MustNotBeEmptyAttribute();
        var model = new TestModel();
        var results = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), results, true);

        // Assert
        result.Should().BeFalse();
        results.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new("error", new List<string> { "List" })
        });
    }

    [TestMethod]
    public void MustNotBeEmptyValidation_CollectionNotEmpty_ReturnTrue()
    {
        // Arrange
        var sut = new MustNotBeEmptyAttribute();

        // Act
        var result = sut.IsValid(new List<object>{new {}});

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void MustNotBeEmptyValidation_NotAList_ReturnTrue()
    {
        // Arrange
        var sut = new MustNotBeEmptyAttribute();

        // Act
        var result = sut.IsValid(new object());

        // Assert
        result.Should().BeTrue();
    }
}