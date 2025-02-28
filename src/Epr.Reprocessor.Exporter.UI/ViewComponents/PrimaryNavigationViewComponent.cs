using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

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
            var homeLinks = new List<string>
        {
            
        };

            var primaryNavigationModel = new PrimaryNavigationModel();
            primaryNavigationModel.Items = new List<NavigationModel>();

            //var userData = HttpContext.GetUserData();

            if (userData.Id is not null)
            {
                primaryNavigationModel.Items = new List<NavigationModel>
            {
                new()
                {
                    LinkValue = _externalUrlOptions.LandingPage,
                    LocalizerKey = "home",
                    IsActive = homeLinks.Contains(HttpContext.Request.Path.ToString().TrimStart('/'))
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
