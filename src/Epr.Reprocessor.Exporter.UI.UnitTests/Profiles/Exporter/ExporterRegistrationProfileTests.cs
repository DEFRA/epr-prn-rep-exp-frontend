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
            CityOrTown = "Test City",
            StateProvince = "Test State",
            PostCode = "12345",
            CountryName = "Test Country",
            SiteCoordinates = "0,0",
            OverseasAddressContacts = new List<OverseasAddressContact>()
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
            CityOrTown = "Test City",
            StateProvince = "Test State",
            PostCode = "12345",
            CountryName = "Test Country",
            SiteCoordinates = "0,0",
            OverseasAddressContacts = new List<OverseasAddressContact> { contact }
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
            result.OverseasAddressContacts.Should().NotBeNull();
            result.OverseasAddressContacts.Should().HaveCount(1);
            var contact = result.OverseasAddressContacts.First();
            contact.FullName.Should().Be("Jane Smith");
            contact.Email.Should().Be("jane@example.com");
            contact.PhoneNumber.Should().Be("987654321");
        }
    }

    [TestMethod]
    public void Should_Return_Empty_When_OverseasAddressContacts_Is_Null()
    {
        // Arrange
        var address = new OverseasAddress
        {
            OrganisationName = "Test Org",
            OverseasAddressContacts = new List<OverseasAddressContact>(),
            SiteCoordinates = "122",
            AddressLine1 = "address line 1",
            AddressLine2 = "address line 2",
            CityOrTown = "testcity",
            CountryName = "testcountry",
            PostCode = "999999",
            StateProvince = "teststate"
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
    public void Should_Return_Empty_When_OverseasAddressContacts_Is_Empty()
    {
        // Arrange
        var address = new OverseasAddress
        {
            OrganisationName = "Test Org",
            OverseasAddressContacts = new List<OverseasAddressContact>(),
            SiteCoordinates = "122",
            AddressLine1 = "address line 1",
            AddressLine2 = "address line 2",
            CityOrTown = "testcity",
            CountryName = "testcountry",
            PostCode = "999999",
            StateProvince = "teststate"
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
}
