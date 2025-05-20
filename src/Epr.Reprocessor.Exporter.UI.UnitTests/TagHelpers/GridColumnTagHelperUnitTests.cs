namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class GridColumnTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    [DataRow(CommonColumnWidths.OneThird, "govuk-grid-column-one-third")]
    [DataRow(CommonColumnWidths.TwoThirds, "govuk-grid-column-two-thirds")]
    [DataRow(CommonColumnWidths.Full, "govuk-grid-column-full")]
    public void GridColumnTagHelperTagHelper_EnsureGeneratesDivWithGdsStyling(CommonColumnWidths width, string expectedCssClass)
    {
        // Arrange
        var tagHelper = new GridColumnTagHelper()
        {
            ColumnWidth = width
        };

        var tagHelperContext = GenerateTagHelperContext(GridColumnTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(GridColumnTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo($"<div class=\"{expectedCssClass}\"></div>");
    }
}