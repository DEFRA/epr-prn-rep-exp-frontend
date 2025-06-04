namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class RadioButtonContainerTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void RadioButtonContainerTagHelper_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new RadioButtonContainerTagHelper();

        var tagHelperContext = GenerateTagHelperContext(RadioButtonContainerTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(RadioButtonContainerTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-radios\" data-module=\"govuk-radios\"></div>");
    }
}