namespace Epr.Reprocessor.Exporter.UI.UnitTests.Extensions;

using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class HtmlHelperExtensionsTests
{
    private readonly IHtmlHelper _htmlHelper = Mock.Of<IHtmlHelper>();

    [TestMethod]
    public void DisplayMultilineAddress_AllFieldsPresent_ReturnsFormattedHtml()
    {
        // Arrange
        var address = new AddressViewModel
        {
            AddressLine1 = "123 Main St",
            AddressLine2 = "Apt 4B",
            TownOrCity = "Glasgow",
            County = "Lanarkshire",
            Postcode = "G12 8QQ"
        };

        var expected = "123 Main St<br />Apt 4B<br />Glasgow<br />Lanarkshire<br />G12 8QQ";

        // Act
        var result = _htmlHelper.DisplayMultilineAddress(address) as HtmlString;

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }
    }

    [TestMethod]
    public void DisplayMultilineAddress_SomeFieldsNullOrWhitespace_SkipsThem()
    {
        // Arrange
        var address = new AddressViewModel
        {
            AddressLine1 = "123 Main St",
            AddressLine2 = null,
            TownOrCity = "",
            County = "Lanarkshire",
            Postcode = "G12 8QQ"
        };

        var expected = "123 Main St<br />Lanarkshire<br />G12 8QQ";

        // Act
        var result = _htmlHelper.DisplayMultilineAddress(address) as HtmlString;

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }
    }

    [TestMethod]
    public void DisplayMultilineAddress_AllFieldsEmpty_ReturnsEmptyHtmlString()
    {
        // Arrange
        var address = new AddressViewModel();

        // Act
        var result = _htmlHelper.DisplayMultilineAddress(address) as HtmlString;

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(string.Empty);
        }
    }

    [TestMethod]
    public void DisplayMultilineAddress_AddressIsNull_ReturnsEmptyHtmlString()
    {
        // Arrange
        AddressViewModel address = null;

        // Act
        var result = _htmlHelper.DisplayMultilineAddress(address) as HtmlString;

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(string.Empty);
        }
    }

    [TestMethod]
    public void DisplayMultilineAddress_EncodesHtmlCharacters()
    {
        // Arrange
        var address = new AddressViewModel
        {
            AddressLine1 = "<tag>OK</tag>",
            AddressLine2 = null,
            TownOrCity = "Glasgow",
            County = null,
            Postcode = "G1 2AB"
        };

        var expected = "&lt;tag&gt;OK&lt;/tag&gt;<br />Glasgow<br />G1 2AB";

        // Act
        var result = _htmlHelper.DisplayMultilineAddress(address) as HtmlString;

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }
    }
}
