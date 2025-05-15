namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class GridRowTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void GridRowTagHelperTagHelper_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new GridRowTagHelper();

        var tagHelperContext = GenerateTagHelperContext(GridRowTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(GridRowTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-grid-row\"></div>");
    }
}