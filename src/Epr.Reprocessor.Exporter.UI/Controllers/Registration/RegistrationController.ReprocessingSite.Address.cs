using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.Enums;
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
    [Route(PagePaths.AddressOfReprocessingSite)]
    public async Task<IActionResult> AddressOfReprocessingSite()
    {
        var model = new AddressOfReprocessingSiteViewModel();

        // check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);
        if (saveAndContinue is not null && saveAndContinue.Action == nameof(AddressOfReprocessingSite))
        {
            model = JsonConvert.DeserializeObject<AddressOfReprocessingSiteViewModel>(saveAndContinue.Parameters);
            return View(model);
        }

        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite };

        if (session.RegistrationApplicationSession.ReprocessingSite?.TypeOfAddress is not null)
        {
            var reprocessingSite = session.RegistrationApplicationSession.ReprocessingSite;
            model.SetAddress(reprocessingSite.Address, reprocessingSite.TypeOfAddress);
        }
        else
        {
            var organisation = HttpContext.GetUserData().Organisations.FirstOrDefault();

            if (organisation is null)
            {
                throw new ArgumentNullException(nameof(organisation));
            }

            if (organisation.NationId is 0 or null)
            {
                return Redirect(PagePaths.CountryOfReprocessingSite);
            }

            // Not a companies house organisation.
            if (string.IsNullOrEmpty(organisation.CompaniesHouseNumber))
            {
                model = new AddressOfReprocessingSiteViewModel
                {
                    SelectedOption = null,
                    RegisteredAddress = null,
                    BusinessAddress = new AddressViewModel
                    {
                        AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                        AddressLine2 = organisation.Locality,
                        TownOrCity = organisation.Town ?? string.Empty,
                        County = organisation.County ?? string.Empty,
                        Postcode = organisation.Postcode ?? string.Empty
                    }
                };
            }
            // Is a companies house organisation.
            else
            {
                model = new AddressOfReprocessingSiteViewModel
                {
                    SelectedOption = null,
                    BusinessAddress = null,
                    RegisteredAddress = new AddressViewModel
                    {
                        AddressLine1 = $"{organisation.BuildingNumber} {organisation.Street}",
                        AddressLine2 = organisation.Locality,
                        TownOrCity = organisation.Town ?? string.Empty,
                        County = organisation.County ?? string.Empty,
                        Postcode = organisation.Postcode ?? string.Empty
                    }
                };
            }
        }

        await SetTempBackLink(PagePaths.TaskList, PagePaths.AddressOfReprocessingSite);

        await SaveSession(session, PagePaths.AddressOfReprocessingSite, PagePaths.RegistrationLanding);

        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.AddressOfReprocessingSite)]
    public async Task<IActionResult> AddressOfReprocessingSite(AddressOfReprocessingSiteViewModel model)
    {
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.RegistrationLanding, PagePaths.AddressOfReprocessingSite };

        SetBackLink(session, PagePaths.AddressOfReprocessingSite);

        var validationResult = await _validationService.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            ModelState.AddValidationErrors(validationResult);
            return View(model);
        }

        session.RegistrationApplicationSession.ReprocessingSite!.SetAddress(model.GetAddress(), model.SelectedOption);

        await SaveSession(session, PagePaths.AddressOfReprocessingSite, PagePaths.RegistrationLanding);

        await SaveAndContinue(0, nameof(AddressOfReprocessingSite), nameof(RegistrationController), SaveAndContinueAreas.Registration, JsonConvert.SerializeObject(model), SaveAndContinueAddressOfReprocessingSiteKey);

        return Redirect(model.SelectedOption is AddressOptions.RegisteredAddress or AddressOptions.SiteAddress ?
            PagePaths.GridReferenceOfReprocessingSite : PagePaths.CountryOfReprocessingSite);
    }
}