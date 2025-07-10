using Epr.Reprocessor.Exporter.UI.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewComponents;

[TestClass]
public class NotificationBannerTests
{
    private NotificationBanner _viewComponent;

    [TestMethod]
    public void Invoke_ShouldReturnViewWithCorrectMode()
    {
        // Arrange
        var notificationBannerModel = new NotificationBannerModel
        {
            Message = "Test"
        };

        _viewComponent = new NotificationBanner();

        // Act
        var result = _viewComponent.Invoke(notificationBannerModel) as ViewViewComponentResult;

        // Assert
        result.Should().NotBeNull();
        var model = result.ViewData.Model as NotificationBannerModel;
        model.Should().NotBeNull();
        model.Message.Should().Be(notificationBannerModel.Message);
    }
}
