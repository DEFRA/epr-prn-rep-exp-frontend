namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class ButtonGroupTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void ButtonGroupTagHelperTagHelper_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new ButtonGroupTagHelper();

        var tagHelperContext = GenerateTagHelperContext(ButtonGroupTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(ButtonGroupTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-button-group\"></div>");
    }
}