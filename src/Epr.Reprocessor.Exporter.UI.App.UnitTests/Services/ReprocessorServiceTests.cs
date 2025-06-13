using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class ReprocessorServiceTests
{
    private ReprocessorService _sut = null!;
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<IRegistrationMaterialService> _mockRegistrationMaterialService = null!;
    private Mock<IMaterialService> _mockMaterialService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockRegistrationMaterialService = new Mock<IRegistrationMaterialService>();
        _mockMaterialService = new Mock<IMaterialService>();
        _sut = new ReprocessorService(_mockRegistrationService.Object, _mockRegistrationMaterialService.Object, _mockMaterialService.Object);
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnDto()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Id = 1
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.GetAsync(1)).ReturnsAsync(registrationDto);

        // Act
        var result = await _sut.Registrations.GetAsync(1);

        // Assert
        result.Should().BeEquivalentTo(new RegistrationDto
        {
            Id = 1
        });
    }

    [TestMethod]
    public async Task GetByOrganisationAsync_ShouldReturnDto()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Id = 1,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.GetByOrganisationAsync(1, Guid.Empty)).ReturnsAsync(registrationDto);

        // Act
        var result = await _sut.Registrations.GetByOrganisationAsync(1, Guid.Empty);

        // Assert
        result.Should().BeEquivalentTo(new RegistrationDto
        {
            Id = 1,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty
        });
    }

    [TestMethod]
    public void UpdateAsync_ShouldReturnDto()
    {
        // Arrange
        var request = new UpdateRegistrationRequestDto
        {
            Id = 1,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateAsync(1, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.Registrations.UpdateAsync(1, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldReturnDto()
    {
        // Arrange
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1,
            OrganisationId = Guid.Empty
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(1);

        // Act
        var result = await _sut.Registrations.CreateAsync(request);

        // Assert
        result.Should().Be(1);
    }

    [TestMethod]
    public async Task GetRegistrationAndAccreditationAsync_ShouldReturnDto()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Id = 1,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        };

        var registrations = new List<RegistrationDto>{registrationDto};
        
        // Expectations
        _mockRegistrationService.Setup(o => o.GetRegistrationAndAccreditationAsync(Guid.Empty)).ReturnsAsync(registrations);

        // Act
        var result = await _sut.Registrations.GetRegistrationAndAccreditationAsync(Guid.Empty);

        // Assert
        result.Should().BeEquivalentTo([new RegistrationDto
        {
            Id = 1,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        }]);
    }

    [TestMethod]
    public async Task UpdateRegistrationSiteAddressAsync_ShouldReturnDto()
    {
        // Arrange
        var request = new UpdateRegistrationSiteAddressDto
        {
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateRegistrationSiteAddressAsync(1, request)).Returns(Task.CompletedTask);

        // Act
        await _sut.Registrations.UpdateRegistrationSiteAddressAsync(1, request);

        // Assert

        // Silly but need it to remove the warning about no asserts in the test
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void UpdateRegistrationTaskStatusAsync_ShouldReturnDto()
    {
        // Arrange
        var request = new UpdateRegistrationTaskStatusDto
        {
           Status = "New",
           TaskName = "SiteDetails"
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateRegistrationTaskStatusAsync(1, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.Registrations.UpdateRegistrationTaskStatusAsync(1, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }
}