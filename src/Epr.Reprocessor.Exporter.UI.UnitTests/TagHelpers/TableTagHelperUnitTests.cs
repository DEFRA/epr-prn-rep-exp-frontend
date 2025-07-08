namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<table class=\"govuk-table\"></table>");
    }
}