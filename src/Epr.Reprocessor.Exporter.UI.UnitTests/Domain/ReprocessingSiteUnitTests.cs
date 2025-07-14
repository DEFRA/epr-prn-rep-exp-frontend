namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class ReprocessingSiteUnitTests
{
    [TestMethod]
    [DataRow(AddressOptions.RegisteredAddress)]
    [DataRow(AddressOptions.BusinessAddress)]
    [DataRow(AddressOptions.SiteAddress)]
    [DataRow(AddressOptions.DifferentAddress)]
    public void SetAddress_EnsureAddressSet(AddressOptions typeOfAddress)
    {
        // Arrange
        var sut = new ReprocessingSite();

        // Act
        sut.SetAddress(
            new Address("address line 1", "address line 2", "locality", "town", "county", "country", "postcode"),
            typeOfAddress);
        
        // Assert
        sut.Address.Should().BeEquivalentTo(new Address("address line 1", "address line 2", "locality", "town",
            "county", "country", "postcode"));
        sut.TypeOfAddress.Should().Be(typeOfAddress);
    }

    [TestMethod]
    public void SetSiteGridReference_EnsureValueSet()
    {
        // Arrange
        var sut = new ReprocessingSite();

        // Act
        sut.SetSiteGridReference("T12345");

        // Assert
        sut.Should().BeEquivalentTo(new ReprocessingSite
        {
            SiteGridReference = "T12345"
        });
    }

    [TestMethod]
    [DataRow(UkNation.England)]
    [DataRow(UkNation.NorthernIreland)]
    [DataRow(UkNation.Scotland)]
    [DataRow(UkNation.Wales)]
    public void SetNation_EnsureValueSet(UkNation nation)
    {
        // Arrange
        var sut = new ReprocessingSite();

        // Act
        sut.SetNation(nation);

        // Assert
        sut.Should().BeEquivalentTo(new ReprocessingSite
        {
            Nation = nation
        });
    }

    [TestMethod]
    public void SetSourcePage_EnsureValueSet()
    {
        // Arrange
        var sut = new ReprocessingSite();

        // Act
        sut.SetSourcePage("page 1");

        // Assert
        sut.Should().BeEquivalentTo(new ReprocessingSite
        {
            SourcePage = "page 1"
        });
    }

    [TestMethod]
    public void ReprocessingSite_EnsureValuesInitialised()
    {
        // Act
        var sut = new ReprocessingSite();

        // Assert
        sut.Should().BeEquivalentTo(new ReprocessingSite
        {
            Nation = null,
            SiteGridReference = null,
            SourcePage = null,
            Address = null,
            TypeOfAddress = null,
            LookupAddress = new(),
            ServiceOfNotice = new(),
        });
    }
}