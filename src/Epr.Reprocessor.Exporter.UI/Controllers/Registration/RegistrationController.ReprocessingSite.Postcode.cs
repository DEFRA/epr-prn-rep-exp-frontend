using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.PostcodeOfReprocessingSite)]
    public async Task<IActionResult> PostcodeOfReprocessingSite()
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.PostcodeOfReprocessingSite };

        SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);

        session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.PostcodeOfReprocessingSite);
        await SaveSession(session, PagePaths.PostcodeOfReprocessingSite, PagePaths.SelectAddressForReprocessingSite);

        var sessionLookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        var model = new PostcodeOfReprocessingSiteViewModel(sessionLookupAddress?.Postcode);

        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.PostcodeOfReprocessingSite)]
    public async Task<IActionResult> PostcodeOfReprocessingSite(PostcodeOfReprocessingSiteViewModel model)
    {
        var validationResult = await _validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.PostcodeOfReprocessingSite };
        SetBackLink(session, PagePaths.PostcodeOfReprocessingSite);

        var sessionLookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        sessionLookupAddress.Postcode = model.Postcode;

        var addressList = await _postcodeLookupService.GetAddressListByPostcodeAsync(sessionLookupAddress.Postcode);
        var newLookupAddress = new Domain.LookupAddress(model.Postcode, addressList ?? new AddressList(), sessionLookupAddress.SelectedAddressIndex);
        session.RegistrationApplicationSession.ReprocessingSite.LookupAddress = newLookupAddress;

        await SaveSession(session, PagePaths.PostcodeOfReprocessingSite, PagePaths.SelectAddressForReprocessingSite);

        if (addressList is null || !addressList.Addresses.Any())
        {
            return Redirect(PagePaths.NoAddressFound);
        }

        return Redirect(PagePaths.SelectAddressForReprocessingSite);
    }

}