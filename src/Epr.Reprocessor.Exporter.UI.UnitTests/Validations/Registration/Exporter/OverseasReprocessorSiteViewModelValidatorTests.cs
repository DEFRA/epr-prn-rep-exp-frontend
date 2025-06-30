using System.Collections.Generic;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration
{
    [TestClass]
    public class OverseasReprocessorSiteViewModelValidatorTests
    {
        private OverseasReprocessorSiteViewModelValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new OverseasReprocessorSiteViewModelValidator();
        }

        private OverseasReprocessorSiteViewModel CreateValidModel()
        {
            return new OverseasReprocessorSiteViewModel
            {
                Country = "France",
                OrganisationName = "Test Org",
                AddressLine1 = "123 Main St",
                AddressLine2 = "Suite 1",
                CityorTown = "Paris",
                StateProvince = "Ile-de-France",
                Postcode = "75000",
                SiteCoordinates = "48.8566, 2.3522",
                ContactFullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "0123456789",
                Countries = new List<string> { "France", "Germany" },
                IsFirstSite = true
            };
        }

        [TestMethod]
        public void Should_Pass_Validation_For_Valid_Model()
        {
            var model = CreateValidModel();

            var result = _validator.Validate(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void Should_Fail_When_Required_Fields_Are_Empty()
        {
            var model = new OverseasReprocessorSiteViewModel();

            var result = _validator.Validate(model);

            using (new AssertionScope())
            {
                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "Country" && x.ErrorMessage == "Select the country the site is in");
                result.Errors.Should().Contain(x => x.PropertyName == "OrganisationName" && x.ErrorMessage == "Enter the organisation’s name");
                result.Errors.Should().Contain(x => x.PropertyName == "AddressLine1" && x.ErrorMessage == "Enter address line 1, typically the building and street");
                result.Errors.Should().Contain(x => x.PropertyName == "CityorTown" && x.ErrorMessage == "Enter a city or town");
                result.Errors.Should().Contain(x => x.PropertyName == "SiteCoordinates" && x.ErrorMessage == "Enter the latitude and longitude coordinates for the site’s main entrance");
                result.Errors.Should().Contain(x => x.PropertyName == "ContactFullName" && x.ErrorMessage == "Enter the name of the person the regulator can contact");
                result.Errors.Should().Contain(x => x.PropertyName == "Email" && x.ErrorMessage == "Enter the email of the person the regulator can contact");
                result.Errors.Should().Contain(x => x.PropertyName == "PhoneNumber" && x.ErrorMessage == "Enter the phone number of the person the regulator can contact");
            }
        }

        [TestMethod]
        public void Should_Fail_When_Fields_Exceed_Max_Length()
        {
            var model = CreateValidModel();
            model.AddressLine1 = new string('A', 101);
            model.AddressLine2 = new string('B', 101);
            model.CityorTown = new string('C', 71);
            model.StateProvince = new string('D', 71);
            model.Postcode = new string('E', 21);
            model.SiteCoordinates = new string('F', 101);
            model.ContactFullName = new string('G', 101);
            model.Email = new string('h', 101) + "@example.com";
            model.PhoneNumber = new string('9', 26);

            var result = _validator.Validate(model);

            using (new AssertionScope())
            {
                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "AddressLine1");
                result.Errors.Should().Contain(x => x.PropertyName == "AddressLine2");
                result.Errors.Should().Contain(x => x.PropertyName == "CityorTown");
                result.Errors.Should().Contain(x => x.PropertyName == "StateProvince");
                result.Errors.Should().Contain(x => x.PropertyName == "Postcode");
                result.Errors.Should().Contain(x => x.PropertyName == "SiteCoordinates");
                result.Errors.Should().Contain(x => x.PropertyName == "ContactFullName");
                result.Errors.Should().Contain(x => x.PropertyName == "Email");
                result.Errors.Should().Contain(x => x.PropertyName == "PhoneNumber");
            }
        }

        [TestMethod]
        public void Should_Fail_When_Email_Is_Invalid()
        {
            var model = CreateValidModel();
            model.Email = "not-an-email";

            var result = _validator.Validate(model);

            using (new AssertionScope())
            {
                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "Email" && x.ErrorMessage == "Enter an email address in the correct format, like name@example.com");
            }
        }
    }
}
