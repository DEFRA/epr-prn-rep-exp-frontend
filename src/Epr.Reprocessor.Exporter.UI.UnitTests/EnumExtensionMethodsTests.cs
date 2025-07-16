using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;

namespace Epr.Reprocessor.Exporter.UI.UnitTests;

[TestClass]
public class EnumExtensionMethodsTests
{
	[TestMethod]
	public void GetDescription_ReturnsCorrectDescription_WhenDescriptionAttributeExists()
	{
		// Arrange
		var status = TaskStatus.CannotStartYet;
		// Act
		var description = status.GetDescription();
		// Assert
		Assert.AreEqual("CANNOT START YET", description);
	}
	
	[TestMethod]
	public void GetDescription_ReturnsEnumName_WhenDescriptionAttributeDoesNotExist()
	{
		// Arrange
		var status = (TaskStatus)999; // An enum value without a Description attribute
		// Act
		var description = status.GetDescription();
		// Assert
		Assert.AreEqual("999", description);
	}

    [TestMethod]
    public void GetDescription_WithoutDescriptionAttribute_ReturnsEnumName()
    {
        // Arrange
        var value = TaskStatus.None;

        // Act
        var result = value.GetDescription();

        // Assert
        result.Should().Be("None");
    }

    [TestMethod]
    public void GetIntValue()
    {
        // Arrange
        var value = TaskStatus.None;

        // Act
        var result = value.GetIntValue();

        // Assert
        result.Should().Be(0);
    }
}