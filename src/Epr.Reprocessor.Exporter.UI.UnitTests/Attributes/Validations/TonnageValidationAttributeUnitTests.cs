using Epr.Reprocessor.Exporter.UI.Validations.Attributes;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations;

[TestClass]
public class TonnageValidationAttributeUnitTests
{
    [TestMethod]
    public void TonnageValidation_EmptyString_ReturnSuccess()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid(string.Empty);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void TonnageValidation_Null_ReturnSuccess()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid(null);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void TonnageValidation_PositiveNumber_ThousandSeparator_ValidValue_ReturnSuccess()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid("1,000");

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void TonnageValidation_NegativeNumber_ThousandSeparator_InvalidValue_LessThanMinimumAllowed_ReturnNotValid()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid("-1,000");

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void TonnageValidation_PositiveNumber_ThousandSeparator_InvalidValue_MoreThanMaximumAllowed_ReturnNotValid()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid("10,000,001");

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void TonnageValidation_Alphanumeric_InvalidValue_ReturnNotValid()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid("abd");

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void TonnageValidation_ReallyBigNumber_NoThousandSeparator_InvalidValue_MoreThanMaximumAllowed_ReturnNotValid()
    {
        // Arrange
        var sut = new TonnageValidationAttribute();

        // Act
        var result = sut.IsValid("100000000000006454545454545454454");

        // Assert
        result.Should().BeFalse();
    }
}
