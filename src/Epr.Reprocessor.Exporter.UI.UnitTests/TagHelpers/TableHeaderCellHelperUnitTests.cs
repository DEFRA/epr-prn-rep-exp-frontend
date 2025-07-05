namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableHeaderCellHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableHeaderCellTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableHeaderCellTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableHeaderCellTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<th class=\"govuk-table__header\" scope=\"col\"></th>");
    }
}