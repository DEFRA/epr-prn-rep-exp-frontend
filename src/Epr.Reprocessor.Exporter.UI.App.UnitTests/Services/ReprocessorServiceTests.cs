using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class ReprocessorServiceTests
{
    private ReprocessorService _sut = null!;
    private Mock<IRegistrationService> _mockRegistrationService = null!;
    private Mock<IRegistrationMaterialService> _mockRegistrationMaterialService = null!;
    private Mock<IMaterialService> _mockMaterialService = null!;
    private Mock<ILogger<IRegistrationService>> _mockLogger = null!;
    private IRegistrationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockRegistrationMaterialService = new Mock<IRegistrationMaterialService>();
        _mockMaterialService = new Mock<IMaterialService>();
        _sut = new ReprocessorService(_mockRegistrationService.Object, _mockRegistrationMaterialService.Object, _mockMaterialService.Object);
        _mockLogger = new Mock<ILogger<IRegistrationService>>();
        _service = _mockRegistrationService.Object;
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var registrationDto = new RegistrationDto
        {
            Id = id
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.GetAsync(id)).ReturnsAsync(registrationDto);

        // Act
        var result = await _sut.Registrations.GetAsync(id);

        // Assert
        result.Should().BeEquivalentTo(new RegistrationDto
        {
            Id = id
        });
    }

    [TestMethod]
    public async Task GetByOrganisationAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var registrationDto = new RegistrationDto
        {
            Id = id,
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
            Id = id,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty
        });
    }

    [TestMethod]
    public void UpdateAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationRequestDto
        {
            RegistrationId = id,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateAsync(id, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.Registrations.UpdateAsync(id, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1,
            OrganisationId = Guid.Empty
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(new CreateRegistrationResponseDto
        {
            Id = id
        });

        // Act
        var result = await _sut.Registrations.CreateAsync(request);

        // Assert
        result.Should().BeEquivalentTo(new CreateRegistrationResponseDto
        {
            Id = id
        });
    }

    [TestMethod]
    public async Task GetRegistrationAndAccreditationAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var registrationDto = new RegistrationDto
        {
            Id = id,
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
            Id = id,
            ApplicationTypeId = ApplicationType.Reprocessor,
            OrganisationId = Guid.Empty,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        }]);
    }

    [TestMethod]
    public void UpdateRegistrationSiteAddressAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationSiteAddressDto
        {
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1"
            }
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateRegistrationSiteAddressAsync(id, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.Registrations.UpdateRegistrationSiteAddressAsync(id, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }

    [TestMethod]
    public void UpdateRegistrationTaskStatusAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationTaskStatusDto
        {
           Status = "New",
           TaskName = "SiteDetails"
        };

        // Expectations
        _mockRegistrationService.Setup(o => o.UpdateRegistrationTaskStatusAsync(id, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.Registrations.UpdateRegistrationTaskStatusAsync(id, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }

    [TestMethod]
    public void UpdateApplicationRegistrationTaskStatusAsync_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationTaskStatusDto
        {
            Status = "New",
            TaskName = "SiteDetails"
        };

        // Expectations
        _mockRegistrationMaterialService.Setup(o => o.UpdateApplicationRegistrationTaskStatusAsync(id, request)).Returns(Task.CompletedTask);

        // Act
        var result = _sut.RegistrationMaterials.UpdateApplicationRegistrationTaskStatusAsync(id, request);

        // Assert
        result.Should().Be(Task.CompletedTask);
    }
}