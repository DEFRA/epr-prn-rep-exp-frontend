using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace Epr.Reprocessor.Exporter.UI.ViewComponents;

public class LanguageSwitcherViewComponent : ViewComponent
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public LanguageSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions, IFeatureManager featureManager)
    {
        _localizationOptions = localizationOptions;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        var languageSwitcherModel = new LanguageSwitcherModel
        {
            SupportedCultures = _localizationOptions.Value.SupportedCultures!.ToList(),
            CurrentCulture = cultureFeature!.RequestCulture.Culture,
            ReturnUrl = $"~{Request.Path}{Request.QueryString}",
            ShowLanguageSwitcher = true
        };

        return View(languageSwitcherModel);
    }
}