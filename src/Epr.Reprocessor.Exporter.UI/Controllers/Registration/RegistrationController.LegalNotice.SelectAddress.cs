using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.SelectAddressForServiceOfNotices)]
    public async Task<IActionResult> SelectAddressForServiceOfNotices()
    {
        var model = GetStubDataFromTempData<SelectAddressForServiceOfNoticesViewModel>(SaveAndContinueSelectAddressForServiceOfNoticesKey)
                    ?? new SelectAddressForServiceOfNoticesViewModel();

        // TEMP 
        if (model.Addresses?.Count == 0)
        {
            model.Postcode = "G5 0US";
            model.SelectedIndex = null;
            model.Addresses = GetListOfAddresses(model.Postcode);
        }

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForServiceOfNotices };

        SetBackLink(session, PagePaths.SelectAddressForServiceOfNotices);

        await SaveSession(session, PagePaths.SelectAddressForServiceOfNotices, PagePaths.RegistrationLanding);

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.SelectAddressForServiceOfNotices))
        {
            model = JsonConvert.DeserializeObject<SelectAddressForServiceOfNoticesViewModel>(saveAndContinue.Parameters);
        }

        return View(nameof(SelectAddressForServiceOfNotices), model);
    }

    [HttpGet]
    [Route(PagePaths.SelectedAddressForServiceOfNotices)]
    public async Task<IActionResult> SelectedAddressForServiceOfNotices([FromQuery] SelectedAddressViewModel selectedAddress)
    {
        var model = GetStubDataFromTempData<SelectAddressForServiceOfNoticesViewModel>(SaveAndContinueSelectAddressForServiceOfNoticesKey)
                    ?? new SelectAddressForServiceOfNoticesViewModel();

        model.SelectedIndex = selectedAddress.SelectedIndex;

        // TEMP 
        if (model.Addresses?.Count == 0)
        {
            model.Postcode = string.IsNullOrWhiteSpace(selectedAddress.Postcode) ? "G5 0US" : selectedAddress.Postcode;
            model.Addresses = GetListOfAddresses(model.Postcode);
        }

        var validationResult = await _validationService.ValidateAsync(selectedAddress);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(nameof(SelectAddressForServiceOfNotices), model);
        }

        var buttonAction = "SaveAndContinue";

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.SelectAddressForServiceOfNotices };

        SetBackLink(session, PagePaths.SelectAddressForServiceOfNotices);

        await SaveSession(session, PagePaths.SelectAddressForServiceOfNotices, PagePaths.RegistrationLanding);

        await SaveAndContinue(0, nameof(ManualAddressForServiceOfNotices), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueSelectAddressForServiceOfNoticesKey);

        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(PagePaths.RegistrationLanding);
        }
        else if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(PagePaths.ApplicationSaved);
        }

        return View(model);
    }
}