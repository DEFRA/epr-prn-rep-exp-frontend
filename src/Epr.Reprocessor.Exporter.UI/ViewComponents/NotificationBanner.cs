namespace Epr.Reprocessor.Exporter.UI.ViewComponents;

public class NotificationBanner : ViewComponent
{
    public IViewComponentResult Invoke(NotificationBannerModel model)
    {
        return View(model);
    }
}
