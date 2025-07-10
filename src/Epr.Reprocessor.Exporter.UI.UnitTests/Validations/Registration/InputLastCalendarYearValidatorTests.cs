using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class InputLastCalendarYearValidatorTests
{
    private ReprocessingInputsValidator _validator = null!;
    private RawMaterialRowValidator _rawMaterialvalidator = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new ReprocessingInputsValidator();
        _rawMaterialvalidator = new RawMaterialRowValidator();
    }

    [TestMethod]
    [DataRow("1", null, null)]
    [DataRow(null, "1", null)]
    [DataRow(null, null, "1")]
    public void ShouldNotHaveError_When_ValidDataProvided(string? ukPackagingWaste, string? nonUkPackagingWaste, string? nonPackagingWaste)
    {
        // Arrange
        var model = new ReprocessingInputsViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    [DataRow("1,000", null, null)]
    [DataRow(null, "1,000", null)]
    [DataRow(null, null, "1,000")]
    public void ShouldNotHaveError_When_ValidDataProvidedWithCommas(string? ukPackagingWaste, string? nonUkPackagingWaste, string? nonPackagingWaste)
    {
        // Arrange
        var model = new ReprocessingInputsViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_NoValuesForPackagingAndNonPackagingWaste()
    {
        // Arrange
        var model = new ReprocessingInputsViewModel { UkPackagingWaste = null, NonUkPackagingWaste = null, NonPackagingWaste = null };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter a tonnage greater than 0 in at least one of the waste tonnage boxes");
    }

    [TestMethod]
    [DataRow("-1", null, null)]
    [DataRow(null, "-1", null)]
    [DataRow(null, null, "-1")]
    [DataRow("0", null, null)]
    [DataRow(null, "10000001", null)]
    public void ShouldHaveError_When_InvalidRangeForPackagingAndNonPackagingWaste(string? ukPackagingWaste, string? nonUkPackagingWaste, string? nonPackagingWaste)
    {
        // Arrange
        var model = new ReprocessingInputsViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Weight must be more than 0 and not greater than 10,000,000 tonnes");
    }

    [TestMethod]
    [DataRow("1!", null, null)]
    [DataRow(null, "1!", null)]
    [DataRow(null, null, "1!")]
    [DataRow("1!", "1", null)] // Mix of valid and invalid
    public void ShouldHaveError_When_SpecialCharacterForPackagingAndNonPackagingWaste(string? ukPackagingWaste, string? nonUkPackagingWaste, string? nonPackagingWaste)
    {
        // Arrange
        var model = new ReprocessingInputsViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter tonnages in whole numbers, like 10");
    }

    [TestMethod]
    [DataRow("", "12")]
    public void ShouldHaveError_When_RawMaterialBlankAndTonnageHaveValidValue(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage};

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter the name of a raw material");
    }

    [TestMethod]
    [DataRow("Raw11", "")]
    [DataRow("Raw11", "10")]
    [DataRow("Raw11", "10,000")]
    public void ShouldHaveError_When_RawMaterialNameInvalidOtherThanLetters(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage };

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Raw materials must be written using letters");
    }

    [TestMethod]
    [DataRow("RawmaterialSteellRawmaterialSteellRawmaterialSteell", "")]
    public void ShouldHaveError_When_RawMaterialNameInvalidMoreThan50Characters(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage };

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Raw materials must be less than 50 characters");
    }

    [TestMethod]
    [DataRow("Raw", "")]
    [DataRow("RawMaterialA", null)]
    public void ShouldHaveError_When_RawMaterialHaveValidValueAndTonnageBlank(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage };

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter a tonnage for the raw material");
    }

    [TestMethod]
    [DataRow("RawMaterialB", "1!")]
    [DataRow("RawMaterialC", "1.0")]
    public void ShouldHaveError_When_RawMaterialHaveValidValueAndTonnageInvalid(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage };

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter tonnages in whole numbers, like 10");
    }

    [TestMethod]
    [DataRow("RawMaterialB", "0")]
    [DataRow("RawMaterialC", "10000001")]
    public void ShouldHaveError_When_RawMaterialHaveValidValueAndTonnageInvalidRange(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new RawMaterialRowViewModel { RawMaterialName = materialName, Tonnes = tonnage };

        // Act
        var result = _rawMaterialvalidator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Weight must be more than 0 and not greater than 10,000,000 tonnes");
    }

    [TestMethod]
    [DataRow("Plastic", "")]
    public void ShouldTriggerRawMaterialValidation_When_RawMaterialNameIsProvided(string? materialName, string? tonnage)
    {
        // Arrange
        var model = new ReprocessingInputsViewModel
        {
            RawMaterials = new List<RawMaterialRowViewModel>
        {
            new RawMaterialRowViewModel
            {
                RawMaterialName = materialName,
                Tonnes = tonnage 
            }
        }
        };

        //var validator = new InputsForLastCalendarYearValidator();

        // Act
        var result = _validator.Validate(model);
        
        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName.Contains("Tonnes")));
    }

}