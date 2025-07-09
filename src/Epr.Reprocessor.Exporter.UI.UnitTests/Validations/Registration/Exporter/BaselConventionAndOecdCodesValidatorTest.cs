using Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration.Exporter;

[TestClass]
public class BaselConventionAndOecdCodesValidatorTest
{
    private BaselConventionAndOecdCodesValidator _baselConventionAndOecdCodesValidator = null!;

    [TestInitialize]
    public void Setup()
    {
        _baselConventionAndOecdCodesValidator = new BaselConventionAndOecdCodesValidator();
    }

    [TestMethod]
    public void Should_Have_Validation_Error_When_All_OecdCodes_Are_Empty()
    {
        //arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
            {
                new OverseasAddressWasteCodesViewModel
                {
                    CodeName = ""
                },
                new OverseasAddressWasteCodesViewModel
                {
                    CodeName = " "
                },
                new OverseasAddressWasteCodesViewModel
                {
                    CodeName = null
                }
            },
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        //act
        var result = _baselConventionAndOecdCodesValidator.TestValidate(model);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.OecdCodes);
    }

    [TestMethod]
    public void Should_Not_Have_Validation_Error_When_AtLeast_One_OecdCode_Is_Entered()
    {
        //arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
            {
                new OverseasAddressWasteCodesViewModel { CodeName = "" },
                new OverseasAddressWasteCodesViewModel { CodeName = "B01234" }
            },
            OrganisationName = "xyz ltd",
            AddressLine1 = "Address line 1"
        };

        //act
        var result = _baselConventionAndOecdCodesValidator.TestValidate(model);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.OecdCodes);
    }

    [TestMethod]
    public void Should_Have_Validation_Error_When_One_OecdCode_Provided_Is_Null()
    {
        //arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
            {
                new OverseasAddressWasteCodesViewModel { CodeName = null },

            },
            OrganisationName = "xyz ltd",
            AddressLine1 = "Address line 1"
        };

        //act
        var result = _baselConventionAndOecdCodesValidator.TestValidate(model);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.OecdCodes);
    }
}