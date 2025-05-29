using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet(PagePaths.ConfirmNoticesAddress)]
    public IActionResult ConfirmNoticesAddress()
    {
        var model = new ConfirmNoticesAddressViewModel();
        SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);
        return View(model);
    }

    [HttpPost(PagePaths.ConfirmNoticesAddress)]
    public IActionResult ConfirmNoticesAddress(ConfirmNoticesAddressViewModel model)
    {
        SetTempBackLink(PagePaths.SelectAddressForServiceOfNotices, PagePaths.ConfirmNoticesAddress);
        return View(model);
    }

}