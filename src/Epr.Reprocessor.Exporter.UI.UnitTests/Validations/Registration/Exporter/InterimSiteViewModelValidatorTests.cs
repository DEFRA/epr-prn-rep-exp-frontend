using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration.Exporter
{
    [TestClass]
    public class InterimSiteViewModelValidatorTests
    {
        private InterimSiteViewModelValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new InterimSiteViewModelValidator();
        }

        [TestMethod]
        public void Should_Have_Error_When_Country_Is_Empty()
        {
            var model = new InterimSiteViewModel { Country = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.Country)
                    .WithErrorMessage(InterimSiteDetails.CountryRequired);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_OrganisationName_Is_Empty()
        {
            var model = new InterimSiteViewModel { OrganisationName = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.OrganisationName)
                    .WithErrorMessage(InterimSiteDetails.OrganisationNameRequired);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_AddressLine1_Is_Empty_Or_TooLong()
        {
            var model = new InterimSiteViewModel { AddressLine1 = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.AddressLine1)
                    .WithErrorMessage(InterimSiteDetails.AddressLine1Required);
            }

            model.AddressLine1 = new string('a', 101);
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.AddressLine1)
                    .WithErrorMessage(InterimSiteDetails.AddressLine1MaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_AddressLine2_TooLong()
        {
            var model = new InterimSiteViewModel { AddressLine2 = new string('a', 101) };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.AddressLine2)
                    .WithErrorMessage(InterimSiteDetails.AddressLine2MaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_CityorTown_Is_Empty_Or_TooLong()
        {
            var model = new InterimSiteViewModel { CityorTown = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.CityorTown)
                    .WithErrorMessage(InterimSiteDetails.CityorTownRequired);
            }

            model.CityorTown = new string('a', 71);
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.CityorTown)
                    .WithErrorMessage(InterimSiteDetails.CityorTownMaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_StateProvince_TooLong()
        {
            var model = new InterimSiteViewModel { StateProvince = new string('a', 71) };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.StateProvince)
                    .WithErrorMessage(InterimSiteDetails.StateProvinceMaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_Postcode_TooLong()
        {
            var model = new InterimSiteViewModel { Postcode = new string('a', 21) };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.Postcode)
                    .WithErrorMessage(InterimSiteDetails.PostcodeMaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_ContactFullName_Is_Empty_Or_TooLong()
        {
            var model = new InterimSiteViewModel { ContactFullName = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.ContactFullName)
                    .WithErrorMessage(InterimSiteDetails.ContactFullNameRequired);
            }

            model.ContactFullName = new string('a', 101);
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.ContactFullName)
                    .WithErrorMessage(InterimSiteDetails.ContactFullNameMaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_Email_Is_Empty_Invalid_Or_TooLong()
        {
            var model = new InterimSiteViewModel { Email = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.Email)
                    .WithErrorMessage(InterimSiteDetails.EmailRequired);
            }

            model.Email = "invalid-email";
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.Email)
                    .WithErrorMessage(InterimSiteDetails.EmailInvalid);
            }

            model.Email = new string('a', 101) + "@test.com";
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.Email)
                    .WithErrorMessage(InterimSiteDetails.EmailMaxLength);
            }
        }

        [TestMethod]
        public void Should_Have_Error_When_PhoneNumber_Is_Empty_Or_TooLong()
        {
            var model = new InterimSiteViewModel { PhoneNumber = "" };
            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                    .WithErrorMessage(InterimSiteDetails.PhoneNumberRequired);
            }

            model.PhoneNumber = new string('1', 26);
            result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                    .WithErrorMessage(InterimSiteDetails.PhoneNumberMaxLength);
            }
        }

        [TestMethod]
        public void Should_Not_Have_Errors_When_Model_Is_Valid()
        {
            var model = new InterimSiteViewModel
            {
                Country = "UK",
                OrganisationName = "Org",
                AddressLine1 = "Line1",
                AddressLine2 = "Line2",
                CityorTown = "Town",
                StateProvince = "State",
                Postcode = "12345",
                ContactFullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "0123456789"
            };

            var result = _validator.TestValidate(model);

            using (new AssertionScope())
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
        }
    }
}
