namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TableCaptionTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void Process_NotHidden_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableCaptionTagHelper();

        var tagHelperContext = GenerateTagHelperContext(TableCaptionTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableCaptionTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<caption class=\"govuk-table__caption govuk-table__caption--m\"></caption>");
    }

    [TestMethod]
    public void Process_Hidden_EnsureCorrectHtmlWithGdsStyling()
    {
        // Arrange
        var tagHelper = new TableCaptionTagHelper
        {
            IsHidden = true
        };

        var tagHelperContext = GenerateTagHelperContext(TableCaptionTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(TableCaptionTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<caption class=\"govuk-table__caption govuk-table__caption--m govuk-visually-hidden\"></caption>");
    }
}