using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class TaskListStatusTagTagHelperUnitTests : TagHelpersUnitTestBase
{
    [TestMethod]
    [DataRow(TaskStatus.CannotStartYet, "govuk-tag--grey")]
    [DataRow(TaskStatus.NotStart, "govuk-tag--grey")]
    [DataRow(TaskStatus.InProgress, "govuk-tag--blue")]
    [DataRow(TaskStatus.Completed, "govuk-tag--green")]
    public void Process_EnsureCorrectAdditionalCssClasses(TaskStatus status, string expectedCssClass)
    {
        // Arrange
        var sut = new TaskListStatusTagTagHelper
        {
            Status = status
        };

        var tagHelperContext = GenerateTagHelperContext("strong");
        var tagHelperOutput = GenerateTagHelperOutput("strong", new TagHelperAttributeList());

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes["class"].Value.ToString().Should().BeEquivalentTo($"govuk-tag {expectedCssClass}");
    }

    [TestMethod]
    public void Process_ArgumentOutOfRangeException_EnsureCorrectAdditionalCssClasses()
    {
        // Arrange
        var sut = new TaskListStatusTagTagHelper
        {
            Status = (TaskStatus)16 // Invalid status to trigger exception
        };

        var tagHelperContext = GenerateTagHelperContext("strong");
        var tagHelperOutput = GenerateTagHelperOutput("strong", new TagHelperAttributeList());

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.Process(tagHelperContext, tagHelperOutput));
    }
}