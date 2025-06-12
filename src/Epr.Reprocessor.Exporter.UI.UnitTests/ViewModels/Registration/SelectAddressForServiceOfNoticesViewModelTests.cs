namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class SelectAddressForServiceOfNoticesViewModelTests
{
    [TestMethod]
    public void TestFormattedAddressProperty()
    {
        // Arrange
        var model = new AddressViewModel
        {
            AddressLine1 = "Address Line 1",
            AddressLine2 = "Address Line 2",
            TownOrCity = "City",
            County = "County",
            Postcode = "P01 4BG"
        };

        var expectedAddress = "Address Line 1, Address Line 2, City, County, P01 4BG";

        // Act
        var actualAddress = model.FormattedAddress;

        // Assert
        actualAddress.Should().Be(expectedAddress);
    }
}