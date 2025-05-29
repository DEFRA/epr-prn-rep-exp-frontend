using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public partial class RegistrationController
{
    [HttpGet]
    [Route(PagePaths.AddressForNotices)]
    public async Task<IActionResult> AddressForNotices()
    {
        var model = new AddressForNoticesViewModel
        {
            AddressToShow = new AddressViewModel
            {
                AddressLine1 = "23 Ruby Street",
                AddressLine2 = "",
                TownOrCity = "London",
                County = "UK",
                Postcode = "EE12 345"
            }
        };
        var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
        session.Journey = new List<string> { PagePaths.AddressForNotices, PagePaths.AddressForNotices };

        SetBackLink(session, PagePaths.AddressForNotices);

        await SaveSession(session, PagePaths.GridReferenceOfReprocessingSite, PagePaths.AddressForNotices);

        //check save and continue data
        var saveAndContinue = await GetSaveAndContinue(0, nameof(RegistrationController), SaveAndContinueAreas.Registration);

        GetStubDataFromTempData<AddressForNoticesViewModel>(SaveAndContinueAddressForNoticesKey);

        if (saveAndContinue is not null && saveAndContinue.Action == nameof(RegistrationController.AddressForNotices))
        {
            model = JsonConvert.DeserializeObject<AddressForNoticesViewModel>(saveAndContinue.Parameters);
        }
        return View(nameof(AddressForNotices), model);
    }

    [HttpPost]
    [Route(PagePaths.AddressForNotices)]
    public async Task<IActionResult> AddressForNotices(AddressForNoticesViewModel model, string buttonAction)
    {
        if (model.SelectedAddressOptions == 0)
        {
            ModelState.AddModelError(nameof(model.SelectedAddressOptions), "Select an address for service of notices.");
        }

        if (!ModelState.IsValid)
        {
            var session = await _sessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorExporterRegistrationSession();
            session.Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.AddressForNotices };

            SetBackLink(session, PagePaths.AddressForNotices);

            model = new AddressForNoticesViewModel
            {
                AddressToShow = new AddressViewModel
                {
                    AddressLine1 = "23 Ruby Street",
                    AddressLine2 = "",
                    TownOrCity = "London",
                    County = "UK",
                    Postcode = "EE12 345"
                }
            };

            return View(nameof(AddressForNotices), model);
        }
        // TODO: Wire up backend / perform next step
        throw new NotImplementedException();

    }
}