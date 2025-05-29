using Epr.Reprocessor.Exporter.UI.App.Clients.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.App.Services;
using FluentAssertions;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.App.Tests.Services;

[TestClass]
public class PostcodeLookupServiceTests
{
    private Mock<IPostcodeLookupApiClient> _mockApiClient = null!;
    private PostcodeLookupService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockApiClient = new Mock<IPostcodeLookupApiClient>();
        _service = new PostcodeLookupService(_mockApiClient.Object);
    }

    [TestMethod]
    public async Task GetAddressListByPostcodeAsync_ShouldReturnAddressList_FromApiClient()
    {
        // Arrange
        var postcode = "G5 5XX";
        var expectedAddressList = new AddressList();
        _mockApiClient
            .Setup(client => client.GetAddressListByPostcodeAsync(postcode))
            .ReturnsAsync(expectedAddressList);

        // Act
        var result = await _service.GetAddressListByPostcodeAsync(postcode);

        // Assert
        result.Should().BeSameAs(expectedAddressList);
        _mockApiClient.Verify(client => client.GetAddressListByPostcodeAsync(postcode), Times.Once);
    }
}
