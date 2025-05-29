using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.SelectAddressForReprocessingSite)]
    public async Task<IActionResult> SelectAddressForReprocessingSite()
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.PostcodeOfReprocessingSite, PagePaths.SelectAddressForReprocessingSite };

        SetBackLink(session, PagePaths.SelectAddressForReprocessingSite);

        session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.SelectAddressForReprocessingSite);

        await SaveSession(session, PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

        var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        var viewModel = new SelectAddressForReprocessingSiteViewModel(lookupAddress);

        return View(nameof(SelectAddressForReprocessingSite), viewModel);
    }

    [HttpGet]
    [Route(PagePaths.SelectedAddressForReprocessingSite)]
    public async Task<IActionResult> SelectedAddressForReprocessingSite([FromQuery] SelectedAddressViewModel selectedAddress)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();

        var lookupAddress = session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
        var viewModel = new SelectAddressForReprocessingSiteViewModel(lookupAddress);

        var validationResult = await _validationService.ValidateAsync(selectedAddress);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(nameof(SelectAddressForReprocessingSite), viewModel);
        }

        session.Journey = new List<string> { PagePaths.PostcodeOfReprocessingSite, PagePaths.SelectAddressForReprocessingSite };

        SetBackLink(session, PagePaths.SelectAddressForReprocessingSite);

        session.RegistrationApplicationSession.ReprocessingSite!.SetSourcePage(PagePaths.SelectedAddressForReprocessingSite);

        lookupAddress.SelectedAddressIndex = selectedAddress.SelectedIndex;
        session.RegistrationApplicationSession.ReprocessingSite.LookupAddress = lookupAddress;

        await SaveSession(session, PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceOfReprocessingSite);

        return Redirect(PagePaths.GridReferenceOfReprocessingSite);
    }

}