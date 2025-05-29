using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.ManualAddressForReprocessingSite)]
    public async Task<IActionResult> ManualAddressForReprocessingSite()
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

        if (reprocessingSite?.TypeOfAddress is null or not AddressOptions.DifferentAddress)
        {
            return Redirect(PagePaths.AddressOfReprocessingSite);
        }

        session.Journey = new List<string> { reprocessingSite.SourcePage, PagePaths.ManualAddressForReprocessingSite };
        SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

        var model = new ManualAddressForReprocessingSiteViewModel();
        var address = reprocessingSite.Address;

        if (address is not null)
        {
            model = new ManualAddressForReprocessingSiteViewModel
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                County = address.County,
                Postcode = address.Postcode,
                TownOrCity = address.Town,
                SiteGridReference = reprocessingSite.SiteGridReference
            };
        }

        await SaveSession(session, PagePaths.ManualAddressForReprocessingSite, PagePaths.AddressForNotices);

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(ManualAddressForReprocessingSite))
        {
            model = JsonConvert.DeserializeObject<ManualAddressForReprocessingSiteViewModel>(saveAndContinue.Parameters);
        }

        return View(nameof(ManualAddressForReprocessingSite), model);
    }

    [HttpPost]
    [Route(PagePaths.ManualAddressForReprocessingSite)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManualAddressForReprocessingSite(ManualAddressForReprocessingSiteViewModel model, string buttonAction)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;

        session.Journey = new List<string> { reprocessingSite!.SourcePage, PagePaths.ManualAddressForReprocessingSite };

        SetBackLink(session, PagePaths.ManualAddressForReprocessingSite);

        var validationResult = await _validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        var country = reprocessingSite.Nation.GetDisplayName();
        session.RegistrationApplicationSession.ReprocessingSite?.SetAddress(
            new Domain.Address(model.AddressLine1,
                model.AddressLine2,
                null,
                model.TownOrCity,
                model.County,
                country,
                model.Postcode),
            AddressOptions.DifferentAddress);

        session.RegistrationApplicationSession.ReprocessingSite?.SetSiteGridReference(model.SiteGridReference);

        await SaveSession(session, PagePaths.ManualAddressForReprocessingSite, PagePaths.AddressForNotices);

        await SaveAndContinue(0, nameof(ManualAddressForReprocessingSite), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueManualAddressForReprocessingSiteKey);

        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(PagePaths.AddressForNotices);
        }

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }
}