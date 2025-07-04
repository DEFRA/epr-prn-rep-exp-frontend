using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Profiles.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Profiles.Exporter;

[TestClass]
public class ExporterRegistrationProfileTests
{
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ExporterRegistrationProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [TestMethod]
    public void Should_Map_OverseasAddress_To_OverseasReprocessorSiteViewModel_With_Empty_Contacts()
    {
        // Arrange
        var address = new OverseasAddress
        {
            OrganisationName = "Test Org",
            AddressLine1 = "Line 1",
            AddressLine2 = "Line 2",
            CityorTown = "Test City",
            StateProvince = "Test State",
            PostCode = "12345",
            Country = "Test Country",
            SiteCoordinates = "0,0",
            OverseasAddressContact = new List<OverseasAddressContact>()
        };

        // Act
        var result = _mapper.Map<OverseasReprocessorSiteViewModel>(address);

        // Assert
        using (new AssertionScope())
        {
            result.ContactFullName.Should().BeEmpty();
            result.Email.Should().BeEmpty();
            result.PhoneNumber.Should().BeEmpty();
        }
    }

    [TestMethod]
    public void Should_Map_OverseasAddress_To_OverseasReprocessorSiteViewModel_With_Contact()
    {
        // Arrange
        var contact = new OverseasAddressContact
        {
            FullName = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "123456789"
        };
        var address = new OverseasAddress
        {
            OrganisationName = "Test Org",
            AddressLine1 = "Line 1",
            AddressLine2 = "Line 2",
            CityorTown = "Test City",
            StateProvince = "Test State",
            PostCode = "12345",
            Country = "Test Country",
            SiteCoordinates = "0,0",
            OverseasAddressContact = new List<OverseasAddressContact> { contact }
        };

        // Act
        var result = _mapper.Map<OverseasReprocessorSiteViewModel>(address);

        // Assert
        using (new AssertionScope())
        {
            result.ContactFullName.Should().Be("John Doe");
            result.Email.Should().Be("john@example.com");
            result.PhoneNumber.Should().Be("123456789");
        }
    }

    [TestMethod]
    public void Should_Map_OverseasReprocessorSiteViewModel_To_OverseasAddress()
    {
        // Arrange
        var viewModel = new OverseasReprocessorSiteViewModel
        {
            ContactFullName = "Jane Smith",
            Email = "jane@example.com",
            PhoneNumber = "987654321"
        };

        // Act
        var result = _mapper.Map<OverseasAddress>(viewModel);

        // Assert
        using (new AssertionScope())
        {
            result.OverseasAddressContact.Should().NotBeNull();
            result.OverseasAddressContact.Should().HaveCount(1);
            var contact = result.OverseasAddressContact.First();
            contact.FullName.Should().Be("Jane Smith");
            contact.Email.Should().Be("jane@example.com");
            contact.PhoneNumber.Should().Be("987654321");
        }
    }
}
