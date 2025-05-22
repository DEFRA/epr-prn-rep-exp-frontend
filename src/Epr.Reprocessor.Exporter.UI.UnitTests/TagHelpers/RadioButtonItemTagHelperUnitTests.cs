namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class RadioButtonItemTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void RadioButtonItemTagHelper_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new RadioButtonItemTagHelper();

        var tagHelperContext = GenerateTagHelperContext(RadioButtonItemTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(RadioButtonItemTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-radios__item\"></div>");
    }
}