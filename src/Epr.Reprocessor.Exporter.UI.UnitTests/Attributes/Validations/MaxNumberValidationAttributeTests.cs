using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Epr.Reprocessor.Exporter.UI.Tests.Attributes.Validations
{
	[TestClass]
	public class MaxNumberValidationAttributeTests
	{
		private readonly MaxNumberValidationAttribute _validationAttribute = new();
		[TestMethod]
		[DataRow("124547854584", false)] // Exceeds MaxCharacters, invalid format
		[DataRow("1245478545", true)]  // Within MaxCharacters, valid format
		public void IsValid_WhenPassedText_ReturnsExpectedResult(string? text, bool expectedResult)
		{
			// Act
			var result = _validationAttribute.IsValid(text);
			// Assert
			result.Should().Be(expectedResult);
		}
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
		public void IsValid_WhenValueExceedsMaxCharactersAndInvalidFormat_ReturnsFailure()
		{
			// Arrange
			var invalidText = "12345678901"; // Exceeds MaxCharacters and invalid format
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
	}
}
