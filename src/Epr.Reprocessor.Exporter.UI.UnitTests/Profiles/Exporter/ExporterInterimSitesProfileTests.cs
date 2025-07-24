using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Profiles.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Profiles.Exporter
{
    [TestClass]
    public class ExporterInterimSitesProfileTests
    {
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ExporterInterimSitesProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [TestMethod]
        public void Map_InterimSiteAddress_WithContact_MapsToViewModel()
        {
            // Arrange
            var contact = new OverseasAddressContact
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };
            var address = new InterimSiteAddress
            {
                AddressLine1 = string.Empty,
                AddressLine2 = string.Empty,
                CityOrTown = string.Empty,
                CountryName = string.Empty,
                OrganisationName = string.Empty,
                PostCode = string.Empty,
                StateProvince = string.Empty,
                InterimAddressContact = new List<OverseasAddressContact> { contact }
            };

            // Act
            var result = _mapper.Map<InterimSiteViewModel>(address);

            // Assert
            using (new AssertionScope())
            {
                result.ContactFullName.Should().Be("John Doe");
                result.Email.Should().Be("john@example.com");
                result.PhoneNumber.Should().Be("123456789");
            }
        }

        [TestMethod]
        public void Map_InterimSiteAddress_WithoutContact_MapsToViewModelWithEmptyStrings()
        {
            // Arrange
            var address = new InterimSiteAddress
            {
                AddressLine1 = string.Empty,
                AddressLine2 = string.Empty,
                CityOrTown = string.Empty,
                CountryName = string.Empty,
                OrganisationName = string.Empty,
                PostCode = string.Empty,
                StateProvince = string.Empty,
                InterimAddressContact = new List<OverseasAddressContact>()
            };

            // Act
            var result = _mapper.Map<InterimSiteViewModel>(address);

            // Assert
            using (new AssertionScope())
            {
                result.ContactFullName.Should().BeEmpty();
                result.Email.Should().BeEmpty();
                result.PhoneNumber.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Map_InterimSiteViewModel_To_InterimSiteAddress_MapsContactFields()
        {
            // Arrange
            var viewModel = new InterimSiteViewModel
            {
                ContactFullName = "Jane Smith",
                Email = "jane@example.com",
                PhoneNumber = "987654321"
            };

            // Act
            var result = _mapper.Map<InterimSiteAddress>(viewModel);

            // Assert
            using (new AssertionScope())
            {
                result.InterimAddressContact.Should().NotBeNull();
                result.InterimAddressContact.Should().HaveCount(1);
                var contact = result.InterimAddressContact.First();
                contact.FullName.Should().Be("Jane Smith");
                contact.Email.Should().Be("jane@example.com");
                contact.PhoneNumber.Should().Be("987654321");
            }
        }
    }
}
