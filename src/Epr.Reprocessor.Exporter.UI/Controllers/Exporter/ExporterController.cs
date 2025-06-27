//using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
//using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
//using Epr.Reprocessor.Exporter.UI.Mapper;
//using Epr.Reprocessor.Exporter.UI.ViewModels.Exporter;

using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Mapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Controllers.Exporter;

[FeatureGate(FeatureFlags.ShowRegistration)]
[Route(PagePaths.RegistrationLanding)]
public class ExporterController : RegistrationControllerBase
{
    public ExporterController(
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        IReprocessorService reprocessorService,
        IPostcodeLookupService postcodeLookupService,
        IValidationService validationService,
        IStringLocalizer<SelectAuthorisationType> selectAuthorisationStringLocalizer,
        IRequestMapper requestMapper
    ) : base(sessionManager, reprocessorService, postcodeLookupService, validationService, selectAuthorisationStringLocalizer, requestMapper)
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route(PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task<IActionResult> CheckOverseasReprocessingSitesAnswers()
    {
        // Replace with actual session implementation
        var session = new ExporterRegistrationApplicationSession();
        session.OverseasReprocessingSites = PopulateOverseasAddresses();

        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.OverseasReprocessingSites);
        return View(model);
    }

    [HttpGet]
    [Route(PagePaths.ChangeOverseasReprocessingSite)]
    public async Task<IActionResult> ChangeOverseasReprocessingSite([FromQuery] int index)
    {
        // Replace with actual session implementation
        var session = new ExporterRegistrationApplicationSession();
        session.OverseasReprocessingSites = PopulateOverseasAddresses();
        var overseasAddress = session.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();
        var siteToChange = overseasAddress[index-1];
        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.OverseasReprocessingSites);
        return View(model);
    }

    [HttpGet]
    [Route(PagePaths.DeleteOverseasReprocessingSite)]
    public async Task<IActionResult> DeleteOverseasReprocessingSite([FromQuery] int index)
    {
        // Replace with actual session implementation
        var session = new ExporterRegistrationApplicationSession();
        session.OverseasReprocessingSites = PopulateOverseasAddresses();
        var overseasAddress = session.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();
        var siteToChange = overseasAddress[index - 1];
        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.OverseasReprocessingSites);
        return View(model);
    }

    [HttpGet]
    [Route(PagePaths.ChangeBaselConvention)]
    public async Task<IActionResult> ChangeBaselConvention([FromQuery] int index)
    {
        // Replace with actual session implementation
        var session = new ExporterRegistrationApplicationSession();
        session.OverseasReprocessingSites = PopulateOverseasAddresses();
        var overseasAddress = session.OverseasReprocessingSites.OverseasAddresses.OrderBy(a => a.OrganisationName).ToList();
        var siteToChange = overseasAddress[index - 1];
        var model = new CheckOverseasReprocessingSitesAnswersViewModel(session.OverseasReprocessingSites);
        return View(model);
    }

    [HttpPost]
    [Route(PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task<IActionResult> CheckOverseasReprocessingSitesAnswers(CheckOverseasReprocessingSitesAnswersViewModel model, string buttonAction)
    {
        if (buttonAction == SaveAndContinueActionKey)
        {
            var a = model;
        }
        return Redirect(PagePaths.RegistrationLanding);
    }

    public static OverseasReprocessingSites PopulateOverseasAddresses()
    {
        return new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress>
            {
                new OverseasAddress
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Suite 100",
                    CityorTown = "London",
                    Country = "United Kingdom",
                    Id = Guid.NewGuid(),
                    OrganisationName = "Acme Reprocessing Ltd",
                    PostCode = "AB12 3CD",
                    SiteCoordinates = "51.5074,-0.1278",
                    StateProvince = "Greater London",
                    IsActive = true,
                    OverseasAddressContact = new List<OverseasAddressContact>
                    {
                        new()
                        {
                            FullName = "Dan Export",
                            Email = "danexport@dan.com",
                            PhoneNumber = "020 1234 5678",
                        }
                    },
                    OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                    {
                        new ()
                        {
                            Id = Guid.NewGuid(),
                            CodeName = "C10512",
                        }
                    }
                },
                new OverseasAddress
                {
                    AddressLine1 = "456 Elm St",
                    AddressLine2 = "Floor 2",
                    CityorTown = "Paris",
                    Country = "France",
                    Id = Guid.NewGuid(),
                    OrganisationName = "Reprocess Europe SARL",
                    PostCode = "75001",
                    SiteCoordinates = "48.8566,2.3522",
                    StateProvince = "Île-de-France",
                    IsActive = false,
                    OverseasAddressContact = new List<OverseasAddressContact>
                    {
                        new()
                        {
                            FullName = "Dan Export",
                            Email = "danexport@dan.com",
                            PhoneNumber = "020 1234 5678",
                        }
                    },
                    OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                    {
                        new ()
                        {
                            Id = Guid.NewGuid(),
                            CodeName = "B1001",
                        }
                    }
                },
                new OverseasAddress
                {
                    AddressLine1 = "456 Elm St",
                    AddressLine2 = "Floor 2",
                    CityorTown = "Paris",
                    Country = "France",
                    Id = Guid.NewGuid(),
                    OrganisationName = "Process Europe SARL",
                    PostCode = "75001",
                    SiteCoordinates = "48.8566,2.3522",
                    StateProvince = "Île-de-France",
                    IsActive = false,
                    OverseasAddressContact = new List<OverseasAddressContact>
                    {
                        new()
                        {
                            FullName = "Dan Export",
                            Email = "danexport@dan.com",
                            PhoneNumber = "020 1234 5678",
                        }
                    },
                    OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                    {
                        new ()
                        {
                            Id = Guid.NewGuid(),
                            CodeName = "B1001",
                        }
                    }
                }
            }
        };
    }
}
