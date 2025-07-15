using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter.Test;
using EPR.Common.Authorization.Sessions;
using Epr.Reprocessor.Exporter.UI.Controllers.Exporter;
using Microsoft.EntityFrameworkCore;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

/// <summary>
/// This controller is used for TESTING PURPOSES ONLY.
/// It allows testers to manually create session state required for the Exporter registration journey.
/// It should NEVER be used in production and must remain protected by a feature flag at all times.
/// Feature flag: TestExporter
/// Route path: /registration (shared with the main ExporterController)
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Test controller only; excluded to prevent noise in code coverage metrics")]
[FeatureGate(FeatureFlags.ShowTestExporter)]
[Route(PagePaths.RegistrationLanding)]
public class TestExporterController(ISessionManager<ExporterRegistrationSession> sessionManager) : Controller
{
    private static readonly Dictionary<Guid, List<Guid>> ValidRegistrationMaterials = BuildDictionary();
    private static readonly Dictionary<string, List<Guid>> ValidMaterialNameByRegisteredMaterialId = BuildMaterialNamesDictionary();


    private static Dictionary<Guid, List<Guid>> BuildDictionary()
    {
        var dict = new Dictionary<Guid, List<Guid>>();

        void Add(string regId, params string[] materialIds)
        {
            if (Guid.TryParse(regId, out var regGuid))
            {
                var validMaterials = materialIds
                    .Where(id => Guid.TryParse(id, out _))
                    .Select(Guid.Parse)
                    .ToList();


                dict[regGuid] = validMaterials;
            }
        }

        // From your screenshot
        Add("F267151B-07F0-43CE-BB5B-37671609EB21",
            "10E3046C-0497-4148-A32D-03DBE78E6EB1",     //Plastic
            "ABEE97C3-D0FA-4C7E-92BD-6C220412CCD6",     //Steel
            "85012628-C3E7-45E4-AAD9-2D27E3C3F3FD");    //Aluminium


        Add("3B90C092-C10E-450A-92AE-F3DF455D2D95",
            "EDB61F6E-D8A5-4C1D-A313-CFB494A0770B",     //Plastic
            "814BE2B5-5B12-475A-A913-6D05CDAAB16F",     //Steel
            "045D59BC-C1B7-4295-8810-66F80D6DB474");    //Aluminium

        Add("9D16DEF0-D828-4800-83FB-2B60907F4163",
            "C902C76A-6A28-42CB-BECE-59DA5661176B",     //Plastic
            "671D62BF-8F17-4CDE-AEC1-3B8CB0237A67",     //Steel
            "86594563-F9C6-488E-B68F-A2CC224906C2");    //Aluminium

        Add("4A48708D-22D5-48F3-BD3F-31F41E3CC7E0",
            "86594563-F9C5-4E6E-B8CF-A3CE22490CC2");

        Add("2A7732CC-EFA5-4B21-9D2C-9ED3D951C2BD",
            "4A4B708D-2D25-48F3-BD3F-31F41E3CE7C0",     //Plastic
            "FDC39D3A-8B43-4616-8417-9605603D99F0",     //Steel     
            "EC6CCF06-2E50-4BAC-9B6B-2FC148A3DA88");    //Aluminium

        return dict;
    }

    private static Dictionary<string, List<Guid>> BuildMaterialNamesDictionary()
    {
        var dict = new Dictionary<string, List<Guid>>();

        void Add(string regId, params string[] materialIds)
        {
            var validMaterials = materialIds
                .Where(id => Guid.TryParse(id, out _))
                .Select(Guid.Parse)
                .ToList();

            dict[regId] = validMaterials;
        }


        Add("Plastic",
            "10E3046C-0497-4148-A32D-03DBE78E6EB1",
            "EDB61F6E-D8A5-4C1D-A313-CFB494A0770B",
            "C902C76A-6A28-42CB-BECE-59DA5661176B",
            "4A4B708D-2D25-48F3-BD3F-31F41E3CE7C0"
            );

        Add("Steel",
            "ABEE97C3-D0FA-4C7E-92BD-6C220412CCD6",
            "814BE2B5-5B12-475A-A913-6D05CDAAB16F",
            "671D62BF-8F17-4CDE-AEC1-3B8CB0237A67",
            "FDC39D3A-8B43-4616-8417-9605603D99F0"
            );

        Add("Aluminium",
            "85012628-C3E7-45E4-AAD9-2D27E3C3F3FD",
            "045D59BC-C1B7-4295-8810-66F80D6DB474",
            "86594563-F9C6-488E-B68F-A2CC224906C2",
            "EC6CCF06-2E50-4BAC-9B6B-2FC148A3DA88"
            );

        return dict;
    }


    [HttpGet("test-setup-session")]
    public IActionResult SetupSession()
    {
        var redirectToAction = HttpContext.Request.Query["RedirectToAction"].ToString();
        if (string.IsNullOrWhiteSpace(redirectToAction))
        {
            redirectToAction = nameof(ExporterController.OverseasSiteDetails);
        }

        return View("~/Views/Registration/Exporter/Test/SetupSession.cshtml", new TestExporterSessionViewModel
        {
            RedirectToAction = redirectToAction,
            //default values for testing
            RegistrationId = "F267151B-07F0-43CE-BB5B-37671609EB21",
            RegistrationMaterialId = "10E3046C-0497-4148-A32D-03DBE78E6EB1",
            MaterialName = "Plastic"
        });
    }
    
    [HttpPost("test-setup-session")]
    public async Task<IActionResult> SetupSession(TestExporterSessionViewModel model)
    {
        if (!Guid.TryParse(model.RegistrationId?.Trim(), out var registrationId) || registrationId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.RegistrationId), "Registration ID must be a valid GUID.");
        }

        if (!Guid.TryParse(model.RegistrationMaterialId?.Trim(), out var materialId) || materialId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.RegistrationMaterialId), "Registration Material ID must be a valid GUID.");
        }

        if (ModelState.IsValid &&
            (!ValidRegistrationMaterials.TryGetValue(registrationId, out var validMaterials) ||
             !validMaterials.Contains(materialId)))
        {
            ModelState.AddModelError(string.Empty, "The registration material ID does not belong to the provided registration ID.");
        }

        if (ModelState.IsValid && (!ValidMaterialNameByRegisteredMaterialId.TryGetValue(model.MaterialName, out var ValidRegisteredMaterials) ||
                                   !ValidRegisteredMaterials.Contains(materialId)))
        {
            ModelState.AddModelError(string.Empty, "The material name does not belong to the provided registration material ID.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Registration/Exporter/Test/SetupSession.cshtml", model);
        }

        var session = new ExporterRegistrationSession
        {
            RegistrationId = registrationId,
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = materialId,
                MaterialName = model.MaterialName,

                //TODO: Remove after testing Interim-Site-Details
                InterimSites = new InterimSites { 
                     OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> {
                        new OverseasMaterialReprocessingSite { IsActive = true, OverseasAddress = new OverseasAddressBase { AddressLine1 = "123", AddressLine2 = "123", CityOrTown = "123", CountryName = "123", OrganisationName = "123", PostCode = "123", StateProvince = "123" } }
                     }
                }
            }
        };

        await SaveSession(session, "test-setup-session");

    

        return RedirectToAction(model.RedirectToAction, "Exporter");
    }

    /// <summary>
    /// Save the current session.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    /// <returns>The completed task.</returns>
    protected async Task SaveSession(ExporterRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await sessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Clears the journey in the session from the current page onwards, effectively resetting the journey for the user.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="currentPagePath"></param>
    protected static void ClearRestOfJourney(ExporterRegistrationSession session, string currentPagePath)
    {
        var index = session.Journey.IndexOf(currentPagePath);

        // this also cover if current page not found (index = -1) then it clears all pages
        session.Journey = session.Journey.Take(index + 1).ToList();
    }

}