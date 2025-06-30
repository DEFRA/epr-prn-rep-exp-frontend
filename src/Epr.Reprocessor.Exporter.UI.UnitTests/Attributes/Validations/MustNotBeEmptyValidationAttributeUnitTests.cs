using Epr.Reprocessor.Exporter.UI.Attributes.Validations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations;

[TestClass]
public class MustNotBeEmptyValidationAttributeUnitTests
{
    [TestMethod]
    public void MustNotBeEmptyValidation_EmptyCollection_ReturnFalse()
    {
        // Arrange
        var sut = new MustNotBeEmptyAttribute();

        // Act
        var result = sut.IsValid(new List<object>());

        // Assert
        result.Should().BeFalse();
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