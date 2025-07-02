using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class InputLastCalendarYearValidatorTests
{
    private InputLastCalenderYearValidator _validator = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _validator = new InputLastCalenderYearValidator();
    }
    
    [TestMethod]
    [DataRow(1, null, null)]
    [DataRow(null, 1, null)]
    [DataRow(null, null, 1)]
    public void ShouldNotHaveError_When_ValidDataProvided(int? ukPackagingWaste, int? nonUkPackagingWaste, int? nonPackagingWaste)
    {
        // Arrange
        var model = new InputLastCalenderYearViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    [DataRow(null, null, null)]
    [DataRow(-1, null, null)]
    [DataRow(null, -1, null)]
    [DataRow(null, null, -1)]
    public void ShouldNotHaveError_When_InvalidValuesForPackagingAndNonPackagingWaste(int? ukPackagingWaste, int? nonUkPackagingWaste, int? nonPackagingWaste)
    {
        // Arrange
        var model = new InputLastCalenderYearViewModel { UkPackagingWaste = ukPackagingWaste, NonUkPackagingWaste = nonUkPackagingWaste, NonPackagingWaste = nonPackagingWaste };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
        result.Errors[0].ErrorMessage.Should().Be("Enter a positive whole number in at least one of the tonnage boxes listed");
    }
}