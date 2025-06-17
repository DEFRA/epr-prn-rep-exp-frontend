using Epr.Reprocessor.Exporter.UI.Attributes.Validations;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Attributes.Validations
{
    [TestClass]
	public class MaxNumberValidationAttributeTests
	{
		private readonly MaxNumberValidationAttribute _validationAttribute = new();

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
        public void IsValid_WhenValueHasNoDigit_ReturnsSuccess()
        {
            // Act
            var result = _validationAttribute.IsValid("TF");
            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
		public void IsValid_WhenValueExceedsMaxCharactersAndInvalidFormat_ReturnsFailure()
		{
			// Arrange
			var invalidText = "TD12345678901787"; // Exceeds MaxCharacters and invalid format
											 // Act
			var result = _validationAttribute.IsValid(invalidText);
			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsValid_WhenValueExceedsMaxCharactersAndValidFormat_ReturnsSuccess()
		{
			// Arrange
			var validText = "1234567890"; // Exactly MaxCharacters and valid format
										  // Act
			var result = _validationAttribute.IsValid(validText);
			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void IsValid_WhenRegexTimeoutOccurs_ReturnsFailure()
		{
            // Arrange
            string longInvalidText = "The quick brown fox jumps over the lazy dog. 6565252662662666662 602565650562652626262";
            // Act
            var result = _validationAttribute.IsValid(longInvalidText);
			// Assert
			result.Should().BeFalse();
		}
	}
}
