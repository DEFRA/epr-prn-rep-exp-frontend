namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations;

[TestClass]
public class ListCannotBeEmptyAttributeUnitTests
{
    [TestMethod]
    public void ListCannotBeEmptyAttribute_Success()
    {
        // Arrange
        var sut = new ListCannotBeEmptyAttribute<string>();

        // Act
        var result = sut.IsValid(new List<string>{"item"});

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void ListCannotBeEmptyAttribute_EmptyList()
    {
        // Arrange
        var sut = new ListCannotBeEmptyAttribute<string>();

        // Act
        var result = sut.IsValid(new List<string>());

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void ListCannotBeEmptyAttribute_Null()
    {
        // Arrange
        var sut = new ListCannotBeEmptyAttribute<string>();

        // Act
        var result = sut.IsValid(null);

        // Assert
        result.Should().BeFalse();
    }
}