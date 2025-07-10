namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableBodyHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableBodyTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableBodyTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableBodyTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<tbody class=\"govuk-table__body\"></tbody>");
    }
}