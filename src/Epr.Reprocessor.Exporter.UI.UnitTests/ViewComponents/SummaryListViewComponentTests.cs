using Epr.Reprocessor.Exporter.UI.ViewComponents;
namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewComponents;

[TestClass]
public class SummaryListViewComponentTests
{
    [TestMethod]
    public void SummaryListViewComponent_EnsureInvocationSuccessful()
    {
        // Arrange
        var model = new SummaryListModel
        {
            Rows =
            [
                new()
                {
                    Key = "key",
                    Value = "value",
                    ChangeLinkHiddenAccessibleText = "change me",
                    ChangeLinkUrl = "change"
                }
            ]
        };

        var viewComponent = new SummaryListViewComponent();

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        Assert.IsNotNull(result);
        result.ViewData!.Model!.Should().NotBeNull();
        Assert.IsNotNull(model);
    }
}