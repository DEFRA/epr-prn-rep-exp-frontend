using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.NoAddressFound)]
    public async Task<IActionResult> NoAddressFound()
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.PostcodeOfReprocessingSite, PagePaths.NoAddressFound };
        SetBackLink(session, PagePaths.NoAddressFound);
        await SaveSession(session, PagePaths.NoAddressFound, PagePaths.PostcodeOfReprocessingSite);

        var postCode = "[TEST POSTCODE REPLACE WITH SESSION]"; // TODO: Get from session

        var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        if (!string.IsNullOrWhiteSpace(lookupAddress.Postcode))
        {
            postCode = lookupAddress.Postcode;
        }

        var model = new NoAddressFoundViewModel { Postcode = postCode };

        return View(model);
    }
}