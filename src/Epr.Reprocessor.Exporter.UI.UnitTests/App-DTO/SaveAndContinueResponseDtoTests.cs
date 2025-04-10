using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.App_DTO;

[TestClass]
public class SaveAndContinueResponseDtoTests
{
    [TestMethod]
    public void Test_Id_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = 123;

        // Act
        dto.Id = expected;
        var actual = dto.Id;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Action_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = "TestAction";

        // Act
        dto.Action = expected;
        var actual = dto.Action;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Controller_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = "TestController";

        // Act
        dto.Controller = expected;
        var actual = dto.Controller;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Parameters_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = "TestParameters";

        // Act
        dto.Parameters = expected;
        var actual = dto.Parameters;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_RegistrationId_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = 456;

        // Act
        dto.Id = expected;
        var actual = dto.Id;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Area_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = "TestArea";

        // Act
        dto.Area = expected;
        var actual = dto.Area;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_CreatedOn_Property()
    {
        // Arrange
        var dto = new SaveAndContinueResponseDto();
        var expected = DateTime.Now;

        // Act
        dto.CreatedOn = expected;
        var actual = dto.CreatedOn;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
