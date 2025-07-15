using Epr.Reprocessor.Exporter.UI.App.Helpers;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Helpers;

[TestClass]
public class RegexHelperTests
{
    [TestMethod]
    [DataRow("abc123", 3)]
    [DataRow("12345", 5)]
    [DataRow("abc", 0)]
    [DataRow("", 0)]
    [DataRow(null, 0)]
    [DataRow("abc123def", 0)]
    [DataRow("test7890", 4)]
    public void CountEndingDigits_ShouldReturnCorrectCount(string input, int expected)
    {
        // Act
        var result = RegexHelper.CountEndingDigits(input);

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    [DataRow("W1A 0AX", true)]
    [DataRow("W1A", false)]
    [DataRow(null, false)]
    [DataRow(" ", false)]
    public void ValidateUKPostcode_ShouldReturnCorrectResult(string postcode, bool expected)
    {
        // Act
        var result = RegexHelper.ValidateUKPostcode(postcode);

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    [DataRow("abc123", true)]
    [DataRow("ABC123", true)]
    [DataRow("123456", true)]
    [DataRow("abcdef", true)]
    [DataRow("ABCDEF", true)]
    [DataRow("abc 123", false)]
    [DataRow("abc-123", false)]
    [DataRow("abc_123", false)]
    [DataRow("", false)]
    [DataRow(" ", false)]
    public void IsAlphaNumeric_ShouldReturnCorrectResult(string input, bool expected)
    {
        // Act
        var result = RegexHelper.IsAlphaNumeric(input);

        // Assert
        result.Should().Be(expected);
    }

    [TestMethod]
    [DataRow("abc123", true)]
    [DataRow("ABC123", true)]
    [DataRow("123456", true)]
    [DataRow("abcdef", false)]
    [DataRow("@ABC$", false)]
    public void CointainsNumber_ShouldReturnCorrectResult(string input, bool expected)
    {
        // Act
        var result = RegexHelper.ContainsNumber(input);

        // Assert
        result.Should().Be(expected);
    }
}
