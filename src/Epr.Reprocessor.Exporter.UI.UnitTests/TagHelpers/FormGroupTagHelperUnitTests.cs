namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class FormGroupTagHelperUnitTests : TagHelpersUnitTestBase
{
    private class TestModel
    {
        public string TestProperty { get; set; } = null!;
    }

    [TestMethod]
    public void FormGroupTagHelper_EnsureGeneratesDivWithGdsStyling_NoFormError()
    {
        // Arrange
        var model = new TestModel()
        {
            TestProperty = "some value"
        };

        SetModelMetadata<TestModel>();

        var tagHelper = new FormGroupTagHelper()
        {
            AspFor = new ModelExpression(
                nameof(model.TestProperty), 
                new ModelExplorer(MockModelMetaDataProvider.Object,
               MockModelMetadata.Object, model)),
            ViewContext = CreateDefaultViewContext()
        };

        var tagHelperContext = GenerateTagHelperContext(FormGroupTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(FormGroupTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-form-group\"></div>");
    }

    [TestMethod]
    public void FormGroupTagHelper_EnsureGeneratesDivWithGdsStyling_WithFormErrors()
    {
        // Arrange
        var model = new TestModel()
        {
            TestProperty = "some value"
        };
        
        SetModelMetadata<TestModel>();
        var viewContext = CreateDefaultViewContext();
        viewContext.ModelState.AddModelError(nameof(model.TestProperty), "error");

        var tagHelper = new FormGroupTagHelper
        {
            AspFor = new ModelExpression(
                nameof(model.TestProperty),
                new ModelExplorer(MockModelMetaDataProvider.Object,
               MockModelMetadata.Object, model)),
            ViewContext = viewContext
        };

        var tagHelperContext = GenerateTagHelperContext(FormGroupTagHelper.TagName);
        var tagHelperOutput = GenerateTagHelperOutput(FormGroupTagHelper.TagName, new TagHelperAttributeList());

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        var writer = new StringWriter();
        tagHelperOutput.WriteTo(writer, HtmlEncoder.Default);

        writer.ToString().Should().BeEquivalentTo("<div class=\"govuk-form-group govuk-form-group--error\"></div>");
    }
}