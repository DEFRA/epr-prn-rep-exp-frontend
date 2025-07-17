namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class CheckAnswersViewModelUnitTests
{
    [TestMethod]
    [DataRow(AddressOptions.BusinessAddress)]
    [DataRow(AddressOptions.RegisteredAddress)]
    [DataRow(AddressOptions.SiteAddress)]
    public void CalculateOriginationPage_Returns_AddressForNotices_For_StandardOptions(AddressOptions typeOfAddress)
    {
        // Arrange
        var notice = new ServiceOfNotice { LookupAddress = null, TypeOfAddress = typeOfAddress};
        var reprocessingSite = new ReprocessingSite
        {
            ServiceOfNotice = notice
        };

        var model = new CheckAnswersViewModel(reprocessingSite);
        
        // Act
        var result = model.CalculateOriginationPage();

        // Assert
        result.Should().BeEquivalentTo(PagePaths.AddressForNotices);
    }

    [TestMethod]
    public void CalculateOriginationPage_Returns_ConfirmNoticesAddress_When_LookupAddressExists()
    {
        // Arrange
        var notice = new ServiceOfNotice
        {
            TypeOfAddress = AddressOptions.DifferentAddress,
            LookupAddress = new LookupAddress
            {
                SelectedAddressIndex = 0,
                AddressesForPostcode =
                    [new("Address line 1", "Address line 2", "locality", "town", "county", "country", "postcode")]
            }
        };

        var reprocessingSite = new ReprocessingSite
        {
            ServiceOfNotice = notice
        };

        var model = new CheckAnswersViewModel(reprocessingSite);

        // Act
        var result = model.CalculateOriginationPage();

        // Assert
        result.Should().BeEquivalentTo(PagePaths.ConfirmNoticesAddress);
    }

    [TestMethod]
    public void CalculateOriginationPage_Returns_ManualAddressForServiceOfNotices_When_LookupAddressIsNull()
    {
        // Arrange
        var notice = new ServiceOfNotice
        {
            TypeOfAddress = AddressOptions.DifferentAddress,
            LookupAddress = null
        };
        var reprocessingSite = new ReprocessingSite
        {
            ServiceOfNotice = notice
        };

        var model = new CheckAnswersViewModel(reprocessingSite);

        // Act
        var result = model.CalculateOriginationPage();

        // Assert
        result.Should().BeEquivalentTo(PagePaths.ManualAddressForServiceOfNotices);
    }

    [TestMethod]
    public void CalculateOriginationPage_Returns_ManualAddressForServiceOfNotices_When_SelectedAddressIsNull()
    {
        // Arrange
        var notice = new ServiceOfNotice
        {
            TypeOfAddress = AddressOptions.DifferentAddress,
            LookupAddress = new LookupAddress
            {
                SelectedAddressIndex = null
            }
        };
        var reprocessingSite = new ReprocessingSite
        {
            ServiceOfNotice = notice
        };

        var model = new CheckAnswersViewModel(reprocessingSite);

        // Act
        var result = model.CalculateOriginationPage();

        // Assert
        result.Should().BeEquivalentTo(PagePaths.ManualAddressForServiceOfNotices);
    }
}