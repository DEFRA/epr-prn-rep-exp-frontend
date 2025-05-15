namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class MainWrapperTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    public void MainWrapperTagHelper_IncludeAdditionalPaddingTrue_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new MainWrapperTagHelper();

        var tagHelperContext = GenerateTagHelperContext(MainWrapperTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(MainWrapperTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<main role=\"main\" id=\"main-content\" class=\"govuk-main-wrapper govuk-!-padding-top-4\"></main>");
    }

    [TestMethod]
    public void MainWrapperTagHelper_IncludeAdditionalPaddingFalse_EnsureGeneratesDivWithGdsStyling()
    {
        // Arrange
        var tagHelper = new MainWrapperTagHelper()
        {
            IncludeAdditionalTopPadding = false
        };

        var tagHelperContext = GenerateTagHelperContext(MainWrapperTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(MainWrapperTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<main role=\"main\" id=\"main-content\" class=\"govuk-main-wrapper\"></main>");
    }
}