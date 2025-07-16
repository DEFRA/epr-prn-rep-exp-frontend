namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class RegistrationApplicationSessionUnitTests
{
    [TestMethod]
    public void EnsurePropertiesInitialised()
    {
        // Arrange
        var sut = new RegistrationApplicationSession();

        // Assert
        sut.ReprocessingSite.Should().NotBeNull();
        sut.WasteDetails.Should().NotBeNull();
        sut.ReprocessingInputsAndOutputs.Should().NotBeNull();
        sut.RegistrationTasks.Should().NotBeNull();
    }
}