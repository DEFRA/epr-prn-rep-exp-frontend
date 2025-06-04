namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels;

[TestClass]
public class AddressViewModelTests
{
    [TestMethod]
    public void GetAddress_NotNull_ReturnMapAddress()
    {
        // Arrange
        var model = new AddressViewModel
        {
            AddressLine1 = "address line 1",
            AddressLine2 = "address line 2",
            TownOrCity = "town",
            County = "county",
            Postcode = "G5 5XX"
        };

        var expectedAddress = new Address("address line 1", "address line 2", null, "town", "county", null, "G5 5XX");

        // Act
        var result = model.GetAddress();

        // Assert
        result.Should().BeEquivalentTo(expectedAddress);
    }
}