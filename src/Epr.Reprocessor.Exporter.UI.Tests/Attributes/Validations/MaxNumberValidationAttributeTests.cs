using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using FluentAssertions;

namespace Epr.Reprocessor.Exporter.UI.Tests.Attributes.Validations
{
    [TestClass]
    public class MaxNumberValidationAttributeTests
    {
        private readonly MaxNumberValidationAttribute _validationAttribute = new();

        [TestMethod]
        [DataRow("124547854584", false)]
        [DataRow("1245478545", true)]
        public void IsValid_WhenPassedText_ReturnsExpectedResult(string? text, bool expectedResult)
        {
            // Act
            var result = _validationAttribute.IsValid(text);

            // Assert
            result.Should().Be(expectedResult);
        }

    }
}
