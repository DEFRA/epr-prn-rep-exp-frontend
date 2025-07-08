
using Epr.Reprocessor.Exporter.UI.Validations.ExporterJourney;
using FluentValidation.TestHelper;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.ExporterJourney;

[TestClass]
public class ManualAddressForServiceOfNoticesValidatorTests
{
    private ManualAddressForServiceOfNoticesValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new ManualAddressForServiceOfNoticesValidator();
    }

    [TestMethod]
    public void Validator_WithValidData_ShouldNotHaveErrors()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "The Cupboard under the Stairs",
            AddressLine2 = "4 Privet Drive",
            TownOrCity = "Little Whinging",
            County = "Surrey",
            Postcode = "HP7 9PP"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validator_WhenRequiredFieldsMissing_ShouldHaveErrors()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.AddressLine1);
        result.ShouldHaveValidationErrorFor(x => x.TownOrCity);
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }

    [TestMethod]
    public void Validator_WhenPostcodeInvalid_ShouldHaveError()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "1 Test",
            TownOrCity = "City",
            Postcode = "InvalidCode"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Postcode)
              .WithErrorMessage("Enter a full UK postcode"); 
    }

    [TestMethod]
    public void Validator_WhenFieldExceedsMaxLength_ShouldHaveError()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = new string('a', 101),
            AddressLine2 = new string('b', 101),
            TownOrCity = new string('c', 101),
            County = new string('d', 101),
            Postcode = "SW1A 1AA"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.AddressLine1);
        result.ShouldHaveValidationErrorFor(x => x.AddressLine2);
        result.ShouldHaveValidationErrorFor(x => x.TownOrCity);
        result.ShouldHaveValidationErrorFor(x => x.County);
    }
}
