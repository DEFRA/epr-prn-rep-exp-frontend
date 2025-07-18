using Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class OutPutLastCalendarYearValidatorTests
{
    private ReprocessingOutputModelValidator _validator = null!;
    private ReprocessedMaterialRawDataValidator _rawMaterialvalidator = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new ReprocessingOutputModelValidator();
        _rawMaterialvalidator = new ReprocessedMaterialRawDataValidator();
    }

    [TestMethod]
    public void Should_Have_Error_When_SentToOtherSiteTonnes_Is_Null()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = null
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Enter a tonnage greater than 0 in at least one of reprocessing tonnage boxes.");
    }

    [TestMethod]
    public void Should_Have_Error_When_SentToOtherSiteTonnes_Is_Text()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "NotANumber"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SentToOtherSiteTonnes)
            .WithErrorMessage("Enter tonnages in whole numbers, like 10.");
    }

    [TestMethod]
    public void Should_Have_Error_When_SentToOtherSiteTonnes_Is_Zero()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "0"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SentToOtherSiteTonnes)
            .WithErrorMessage("Enter a tonnage greater than 0.");
    }

    [TestMethod]
    public void Should_Have_Error_When_SentToOtherSiteTonnes_Exceeds_Maximum()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "10000001"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SentToOtherSiteTonnes)
            .WithErrorMessage("Weight must be 10,000,000 tonnes or less.");
    }

    [TestMethod]
    public void Should_Not_Have_Error_For_Valid_SentToOtherSiteTonnes()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "1000"
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SentToOtherSiteTonnes);
        result.ShouldNotHaveValidationErrorFor(x => x.SentToOtherSiteTonnes);
    }

    [TestMethod]
    public void Should_Validate_ContaminantTonnes_As_Per_Rules()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            ContaminantTonnes = "-5"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ContaminantTonnes)
            .WithErrorMessage("Enter a tonnage greater than 0.");
    }

    [TestMethod]
    public void Should_Validate_ProcessLossTonnes_As_Per_Rules()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            ProcessLossTonnes = "0"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProcessLossTonnes)
            .WithErrorMessage("Enter a tonnage greater than 0.");
    }

    [TestMethod]
    [DataRow(null, "10", "Enter the name of a Product.")]
    [DataRow("InvalidNameInvalidNameInvalidNameInvalidNameInvalidName", "10", "Product must be less than 50 characters.")]
    [DataRow("123InvalidName", "10", "Product must be written using letters.")]
    [DataRow("ValidName", null, "Enter a tonnage for the Product.")]
    [DataRow("ValidName", "0", "Weight must be greater than 0.")]
    [DataRow("ValidName", "10000001", "Weight must be 10,000,000 tonnes or less.")]
    [DataRow("ValidName", "0.5", "Enter tonnages in whole numbers, like 10.")]

    public void Should_Validate_ReprocessedMaterialsRawData(string? inputName, string? inputTonnes, string? expectedError)
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            ProcessLossTonnes = "10",
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
            {
                new ReprocessedMaterialRawDataModel
                {
                    MaterialOrProductName = inputName,
                    ReprocessedTonnes = inputTonnes
                }
            }
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveAnyValidationError();
        var firstError = result.Errors.FirstOrDefault();
        firstError.ErrorMessage.Should().Be(expectedError);
    }
}

