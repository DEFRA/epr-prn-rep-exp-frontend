using Epr.Reprocessor.Exporter.UI.App.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Culture;

[AllowAnonymous]
public class CultureController : Controller
{
    [HttpGet]
    [Route(PagePaths.Culture)]
    public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
    {
        HttpContext.Session.SetString(Language.SessionLanguageKey, culture);
        return LocalRedirect(returnUrl);
    }
}
