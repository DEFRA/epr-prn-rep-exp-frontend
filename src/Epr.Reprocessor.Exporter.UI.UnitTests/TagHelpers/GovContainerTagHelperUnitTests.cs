namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class GovContainerTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void GovContainerTagHelper_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new GovContainerTagHelper();

        var tagHelperContext = GenerateTagHelperContext(GovContainerTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(GovContainerTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-width-container\"></div>");
    }
}