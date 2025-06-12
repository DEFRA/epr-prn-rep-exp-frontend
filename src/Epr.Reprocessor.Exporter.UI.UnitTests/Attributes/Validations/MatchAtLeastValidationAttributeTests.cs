using Epr.Reprocessor.Exporter.UI.Attributes.Validations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations
{
    [TestClass]
    public class MatchAtLeastValidationAttributeTests
    {
        private readonly MatchAtLeastValidationAttribute _validationAttribute = new();

        [TestMethod]
        public void IsValid_WhenValueIsNull_ReturnsSuccess()
        {
            // Act
            var result = _validationAttribute.IsValid(null);
            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValid_WhenValueIsEmptyString_ReturnsSuccess()
        {
            // Act
            var result = _validationAttribute.IsValid(string.Empty);
            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValid_WhenValueBelow4CharactersAndInvalidFormat_ReturnsFailure()
        {
            // Arrange
            var invalidText = "TD18"; 
            var result = _validationAttribute.IsValid(invalidText);
            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValid_WhenValueBelow4DigitAndInvalidFormat_ReturnsFailure()
        {
            // Arrange
            var invalidText = "TD122";
            var result = _validationAttribute.IsValid(invalidText);
            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValid_WhenValueIsValidFormat_ReturnsSuccess()
        {
            // Arrange
            var validText = "TS4521"; 
            var result = _validationAttribute.IsValid(validText);
            // Assert
            result.Should().BeTrue();
        }
    }
}
