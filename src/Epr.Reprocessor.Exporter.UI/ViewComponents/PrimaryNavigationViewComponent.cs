using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.ViewComponents
{
    public class PrimaryNavigationViewComponent : ViewComponent
    {
        private readonly ExternalUrlOptions _externalUrlOptions;
        private readonly FrontEndAccountManagementOptions _frontEndAccountManagementOptions;

        public PrimaryNavigationViewComponent(
            IOptions<ExternalUrlOptions> options,
            IOptions<FrontEndAccountManagementOptions> userAccountManagementOptions)
        {
            _frontEndAccountManagementOptions = userAccountManagementOptions.Value;
            _externalUrlOptions = options.Value;
        }

        public ViewViewComponentResult Invoke()
        {
            var primaryNavigationModel = new PrimaryNavigationModel();
            primaryNavigationModel.Items = new List<NavigationModel>();

            var userData = HttpContext.GetUserData();

            if (userData.Id is not null)
            {
                primaryNavigationModel.Items = new List<NavigationModel>
            {
                new()
                {
                    LinkValue = _externalUrlOptions.LandingPage,
                    LocalizerKey = "home",
                    IsActive = false
                },
                new()
                {
                    LinkValue = $"{_frontEndAccountManagementOptions.BaseUrl}/",
                    LocalizerKey = "manage_account_details",
                }
            };
            }

            return View(primaryNavigationModel);
        }
    }
}
