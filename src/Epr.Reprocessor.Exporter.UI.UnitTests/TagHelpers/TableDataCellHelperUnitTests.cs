namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableDataCellHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableDataCellTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableDataCellTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableDataCellTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<td class=\"govuk-table__cell\"></td>");
    }
}