namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class AddressOfReprocessingSiteViewModelUnitTests
{
    [TestMethod]
    public void Constructor()
    {
        // Arrange
        var sut = new AddressOfReprocessingSiteViewModel();

        // Act & Assert
        sut.Should().NotBeNull();
    }

    [TestMethod]
    public void SetAddress_WhenRegisteredAddress_SetsRegisteredAndClearsBusiness()
    {
        // Arrange
        var viewModel = new AddressOfReprocessingSiteViewModel();
        var address = GetSampleAddress();

        // Act
        var updated = viewModel.SetAddress(address, AddressOptions.RegisteredAddress);

        // Assert
        updated.SelectedOption.Should().Be(AddressOptions.RegisteredAddress);
        updated.RegisteredAddress.Should().NotBeNull();
        updated.BusinessAddress.Should().BeNull();
    }

    [TestMethod]
    public void SetAddress_NullAddress()
    {
        // Arrange
        var viewModel = new AddressOfReprocessingSiteViewModel();

        // Act
        var updated = viewModel.SetAddress(null, null);

        // Assert
        updated.SelectedOption.Should().BeNull();
        updated.RegisteredAddress.Should().BeNull();
        updated.BusinessAddress.Should().BeNull();
    }

    [TestMethod]
    public void SetAddress_WhenBusinessAddress_SetsBusinessAndClearsRegistered()
    {
        // Arrange
        var viewModel = new AddressOfReprocessingSiteViewModel();
        var address = GetSampleAddress();

        // Act
        var updated = viewModel.SetAddress(address, AddressOptions.BusinessAddress);

        // Assert
        updated.SelectedOption.Should().Be(AddressOptions.BusinessAddress);
        updated.BusinessAddress.Should().NotBeNull();
        updated.RegisteredAddress.Should().BeNull();
    }

    [TestMethod]
    public void SetAddress_WhenDifferentAddress_SetsRegisteredAndOption()
    {
        // Arrange
        var viewModel = new AddressOfReprocessingSiteViewModel();
        var address = GetSampleAddress();

        // Act
        var updated = viewModel.SetAddress(address, AddressOptions.DifferentAddress);

        // Assert
        updated.SelectedOption.Should().Be(AddressOptions.DifferentAddress);
        updated.RegisteredAddress.Should().NotBeNull();
        updated.BusinessAddress.Should().BeNull(); // stays unchanged
    }

    [TestMethod]
    public void GetAddress_WhenSelectedIsRegistered_ReturnsMappedRegisteredAddress()
    {
        // Arrange
        var addressViewModel = GetSampleAddressViewModel();
        var viewModel = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = AddressOptions.RegisteredAddress,
            RegisteredAddress = addressViewModel
        };

        // Act
        var address = viewModel.GetAddress();

        // Assert
        address.Should().NotBeNull();
        address.AddressLine1.Should().Be(addressViewModel.AddressLine1);
    }

    [TestMethod]
    public void GetAddress_WhenSelectedIsBusiness_ReturnsMappedBusinessAddress()
    {
        // Arrange
        var addressViewModel = GetSampleAddressViewModel();
        var viewModel = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = AddressOptions.BusinessAddress,
            BusinessAddress = addressViewModel
        };

        // Act
        var address = viewModel.GetAddress();

        // Assert
        address.Should().NotBeNull();
        address.Postcode.Should().Be(addressViewModel.Postcode);
    }

    [TestMethod]
    public void GetAddress_WhenSelectedIsDifferent_UsesFallbackMapping()
    {
        // Arrange
        var businessViewModel = GetSampleAddressViewModel();
        var viewModel = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = AddressOptions.DifferentAddress,
            RegisteredAddress = null,
            BusinessAddress = businessViewModel
        };

        // Act
        var address = viewModel.GetAddress();

        // Assert
        address.Should().NotBeNull();
        address.Town.Should().Be(businessViewModel.TownOrCity);
    }

    private static Address GetSampleAddress() =>
        new("Line 1", "Line 2", null, "City", "County", null, "AB12 3CD");

    private static AddressViewModel GetSampleAddressViewModel() =>
        new()
        {
            AddressLine1 = "Line 1",
            AddressLine2 = "Line 2",
            TownOrCity = "City",
            County = "County",
            Postcode = "AB12 3CD"
        };

}