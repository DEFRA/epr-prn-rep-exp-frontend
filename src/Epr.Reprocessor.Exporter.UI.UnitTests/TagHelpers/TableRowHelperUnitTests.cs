namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableRowHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableRowTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableRowTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableRowTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<tr class=\"govuk-table__row\"></tr>");
    }
}