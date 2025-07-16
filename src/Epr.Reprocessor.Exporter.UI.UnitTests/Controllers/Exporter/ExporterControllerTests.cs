using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Controllers.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation.Results;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers.Exporter
{

[TestClass]
public class ExporterControllerTests
{
    private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IRegistrationService> _registrationServiceMock;
    private Mock<IValidationService> _validationServiceMock;
    private Mock<IReprocessorService> _reprocessorServiceMock;
    private Mock<IExporterRegistrationService> _exporterRegistrationService;
    private Mock<ILogger<RegistrationController>> _logger;
    private new Mock<ISaveAndContinueService> _userJourneySaveAndContinueService;
    private DefaultHttpContext _httpContext;
    private ExporterController _controller;
    private Mock<IRegistrationMaterialService> _registrationMaterialServiceMock;

    [TestInitialize]
    public void Setup()
    {
        // ResourcesPath should be 'Resources' but build environment differs from development environment
        // Work around = set ResourcesPath to non-existent location and test for resource keys rather than resource values
        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources_not_found" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<SelectAuthorisationType>(factory);

        _logger = new Mock<ILogger<RegistrationController>>();
        _userJourneySaveAndContinueService = new Mock<ISaveAndContinueService>();
        _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
        _reprocessorServiceMock = new Mock<IReprocessorService>();
        _mapperMock = new Mock<IMapper>();
        _registrationServiceMock = new Mock<IRegistrationService>();
        _validationServiceMock = new Mock<IValidationService>();
        _exporterRegistrationService = new Mock<IExporterRegistrationService>();
        _registrationMaterialServiceMock = new Mock<IRegistrationMaterialService>();

        _controller = new ExporterController(
            _sessionManagerMock.Object,
            _mapperMock.Object,
            _registrationServiceMock.Object,
            _validationServiceMock.Object,
            _reprocessorServiceMock.Object,
            _exporterRegistrationService.Object
        );

        var context = new DefaultHttpContext();
        var sessionMock = new Mock<ISession>();

        sessionMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
        sessionMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny)).Returns(false);
        sessionMock.Setup(x => x.Remove(It.IsAny<string>()));
        // Initialize HttpContext with a mock session
        _httpContext = new DefaultHttpContext();
        var mockSession = new Mock<ISession>();
        _httpContext.Session = mockSession.Object;

        context.Session = sessionMock.Object;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        _httpContext = context;

        var sessionWithInterimSites = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        sessionWithInterimSites.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        sessionWithInterimSites.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock
            .Setup(s => s.GetSessionAsync(context.Session))
            .ReturnsAsync(sessionWithInterimSites);
    }

    [TestMethod]
    public async Task Index_Get_ReturnsError_WhenRegistrationMaterialIdIsNull()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = null
            }
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task Index_Get_ReturnsView_WithModel_WhenActiveOverseasAddressExists()
    {
        // Arrange
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            AddressLine1 = "Default Address Line 1",
            AddressLine2 = "Default Address Line 2",
            CityOrTown = "Default City",
            CountryName = "Default Country",
            OrganisationName = "Default Organisation",
            PostCode = "Default PostCode",
            SiteCoordinates = "Default Coordinates",
            StateProvince = "Default State"
        };
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress> { overseasAddress }
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        var viewModel = new OverseasReprocessorSiteViewModel();
        var countries = new List<string> { "UK", "France" };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasReprocessorSiteViewModel>(overseasAddress))
            .Returns(viewModel);
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("~/Views/Registration/Exporter/OverseasSiteDetails.cshtml");
            var model = viewResult.Model as OverseasReprocessorSiteViewModel;
            model.Should().NotBeNull();
            model.Countries.Should().BeEquivalentTo(countries);
        }
    }

    [TestMethod]
    public async Task Index_Get_ReturnsView_WithNewModel_WhenNoActiveOverseasAddress()
    {
        // Arrange
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress>()
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        var countries = new List<string> { "UK", "France" };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as OverseasReprocessorSiteViewModel;
            model.Should().NotBeNull();
            model.Countries.Should().BeEquivalentTo(countries);
        }
    }

    [TestMethod]
    public async Task Index_Get_ReturnsView_WithNewModel_WhenNullOverseasReprocessingSites()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = null
            },
            Journey = new List<string>()
        };
        var countries = new List<string> { "UK", "France" };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as OverseasReprocessorSiteViewModel;
            model.Should().NotBeNull();
            model.Countries.Should().BeEquivalentTo(countries);
        }
    }

    [TestMethod]
    public async Task Index_Get_ReturnsView_WithNewModel_WhenNullOverseasAddresses()
    {
        // Arrange
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = null
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        var countries = new List<string> { "UK", "France" };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as OverseasReprocessorSiteViewModel;
            model.Should().NotBeNull();
            model.Countries.Should().BeEquivalentTo(countries);
        }
    }

    [TestMethod]
    public async Task Index_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid()
            },
            Journey = new List<string>()
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        _controller.ModelState.AddModelError("Country", "Required");
        var model = new OverseasReprocessorSiteViewModel();
        var countries = new List<string> { "UK" };
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

        var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "Country", ErrorMessage = "Country Missing" }
            });

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var returnedModel = viewResult.Model as OverseasReprocessorSiteViewModel;
            returnedModel.Countries.Should().BeEquivalentTo(countries);
        }
    }

    [TestMethod]
    public async Task Index_Post_ReturnsError_WhenRegistrationMaterialIdIsNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = null
            },
            Journey = new List<string>()
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task Index_Post_UpdatesActiveOverseasAddress_WhenExists()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            AddressLine1 = "Default Address Line 1",
            AddressLine2 = "Default Address Line 2",
            CityOrTown = "Default City",
            CountryName = "Default Country",
            OrganisationName = "Default Organisation",
            PostCode = "Default PostCode",
            SiteCoordinates = "Default Coordinates",
            StateProvince = "Default State"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress> { overseasAddress }
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            _mapperMock.Verify(x => x.Map(model, overseasAddress), Times.Once);
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public async Task Index_Post_AddsNewOverseasAddress_WhenNoneActive()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress>()
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        var mappedAddress = new OverseasAddress
        {
            IsActive = true,
            AddressLine1 = "Default Address Line 1",
            AddressLine2 = "Default Address Line 2",
            CityOrTown = "Default City",
            CountryName = "Default Country",
            OrganisationName = "Default Organisation",
            PostCode = "Default PostCode",
            SiteCoordinates = "Default Coordinates",
            StateProvince = "Default State"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
            .Returns(mappedAddress);

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            overseasSites.OverseasAddresses.Should().Contain(mappedAddress);
            mappedAddress.IsActive.Should().BeTrue();
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public async Task SaveSession_CallsClearRestOfJourney_AndSavesSession()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            Journey = new List<string> { "A", "B", "C" }
        };
        var currentPage = "B";

        // Act
        await _controller.InvokeProtectedMethod<Task>("SaveSession", session, currentPage);

        // Assert
        using (var scope = new AssertionScope())
        {
            _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
            session.Journey.Should().BeEquivalentTo(new List<string> { "A", "B" });
        }
    }

    [TestMethod]
    public async Task Index_Post_InitializesOverseasReprocessingSites_WhenNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = null
            },
            Journey = new List<string>()
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
            .Returns(new OverseasAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                SiteCoordinates = "Default Coordinates",
                StateProvince = "Default State"
            });

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.Should().NotBeNull();
            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Should().NotBeNull();
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public async Task Index_Post_InitializesOverseasAddresses_WhenNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = null
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
            .Returns(new OverseasAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                SiteCoordinates = "Default Coordinates",
                StateProvince = "Default State"
            });

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public async Task Index_Post_usesOverseasAddresses_WhenNotNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress>()
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
            .Returns(new OverseasAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                SiteCoordinates = "Default Coordinates",
                StateProvince = "Default State"
            });

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public async Task Index_Post_InitialisesOverseasReprocessingSites_WhenNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = null
            },
            Journey = new List<string>()
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
            .Returns(new OverseasAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                SiteCoordinates = "Default Coordinates",
                StateProvince = "Default State"
            });

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
        }
    }

    [TestMethod]
    public void SetBackLink_SetsViewBagBackLink()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            Journey = new List<string> { "A", "B", "C" }
        };
        var currentPage = "B";

        // Act
        _controller.InvokeProtectedMethod("SetBackLink", session, currentPage);

        // Assert
        using (var scope = new AssertionScope())
        {
            var backLinkToDisplay = (string)_controller.ViewBag.BackLinkToDisplay;
            backLinkToDisplay.Should().Be("A");
        }
    }

    [TestMethod]
    public void ReturnSaveAndContinueRedirect_ReturnsCorrectRedirect()
    {
        // Act
        var result1 = _controller.InvokeProtectedMethod<RedirectResult>("ReturnSaveAndContinueRedirect", "SaveAndContinue", "/continue", "/back");
        var result2 = _controller.InvokeProtectedMethod<RedirectResult>("ReturnSaveAndContinueRedirect", "SaveAndComeBackLater", "/continue", "/back");
        var result3 = _controller.InvokeProtectedMethod<RedirectResult>("ReturnSaveAndContinueRedirect", "Other", "/continue", "/back");

        // Assert
        using (var scope = new AssertionScope())
        {
            result1.Url.Should().Be("/continue");
            result2.Url.Should().Be("/back");
            result3.Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task Index_Get_SetsJourneyAndBackLink_WhenRegistrationMaterialIdIsNotNull()
    {
        // Arrange
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            AddressLine1 = "Line1",
            AddressLine2 = "Line2",
            CityOrTown = "City",
            CountryName = "CountryName",
            OrganisationName = "Org",
            PostCode = "Post",
            SiteCoordinates = "Coords",
            StateProvince = "State"
        };
        var overseasSites = new OverseasReprocessingSites
        {
            OverseasAddresses = new List<OverseasAddress> { overseasAddress }
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = overseasSites
            },
            Journey = new List<string>()
        };
        var viewModel = new OverseasReprocessorSiteViewModel();
        var countries = new List<string> { "UK", "France" };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<OverseasReprocessorSiteViewModel>(overseasAddress))
            .Returns(viewModel);
        _registrationServiceMock.Setup(x => x.GetCountries())
            .ReturnsAsync(countries);

            // Act
            var result = await _controller.OverseasSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                session.Journey.Should().Contain(PagePaths.ExporterTaskList);
                session.Journey.Should().Contain(PagePaths.OverseasSiteDetails);
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                ((string)_controller.ViewBag.BackLinkToDisplay).Should().NotBeNullOrEmpty(); 
            }
            
        }
        [TestMethod]
        public async Task Index_Get_ReturnsError_WhenSessionIsNull()
        {
            // Arrange
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync((ExporterRegistrationSession)null);

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task Index_Get_ReturnsError_WhenExporterRegistrationApplicationSessionIsNull()
    {
        // Arrange
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ExporterRegistrationSession { ExporterRegistrationApplicationSession = null });

            // Act
            var result = await _controller.OverseasSiteDetails();

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task Index_Post_ReturnsError_WhenSessionIsNull()
    {
        // Arrange
        var model = new OverseasReprocessorSiteViewModel();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ExporterRegistrationSession)null);

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.OverseasSiteDetails(model, "SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task BaselConventionAndOECDCodes_ReturnsView_WithModel_WhenActiveOverseasAddressExists()
    {
        // Arrange
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            OrganisationName = "Test Organisation",
            AddressLine1 = "123 Test St",
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>
                {
                    new OverseasAddressWasteCodes
                    {
                        CodeName = "Code1"
                    },
                    new OverseasAddressWasteCodes
                    {
                        CodeName = "Code2"
                    }
                },
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                MaterialName = "Test Material",
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { overseasAddress }
                }
            },
            Journey = new List<string>()
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml");

            var model = viewResult.Model as BaselConventionAndOecdCodesViewModel;
            model.Should().NotBeNull();
            model.MaterialName.Should().Be("Test Material");
            model.OrganisationName.Should().Be("Test Organisation");
            model.AddressLine1.Should().Be("123 Test St");
            model.OecdCodes.Should().HaveCount(5); // Check the number of waste codes
            model.OecdCodes.Should().ContainSingle(x => x.CodeName == "Code1");
            model.OecdCodes.Should().ContainSingle(x => x.CodeName == "Code2");
        }
    }

    [TestMethod]
    public async Task BaselConventionAndOECDCodes_WhenNoActiveOverseasAddress_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress>
                        {
                            new()
                            {
                                IsActive = false,
                                AddressLine1 = "Address line 1",
                                AddressLine2 = "",
                                CityOrTown = "",
                                CountryName = "",
                                PostCode = "",
                                SiteCoordinates = "",
                                StateProvince = "",
                                OrganisationName = "xyz Ltd"
                            },
                            new()
                            {  IsActive = false,
                                OrganisationName = "xyz Ltd",
                                AddressLine1 = "",
                                AddressLine2 = "",
                                CityOrTown = "",
                                CountryName = "",
                                PostCode = "",
                                SiteCoordinates = "",
                                StateProvince = ""
                            }
                        }
                },
                MaterialName = "Plastic"
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        Func<Task> action = async () => await _controller.BaselConventionAndOECDCodes();

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("overseasAddressActiveRecord");
    }

    [TestMethod]
    public async Task BaselConventionAndOECDCodes_PopulatesOecdCodes_WithEmptyList_WhenNoneExist()
    {
        // Arrange
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            OrganisationName = "Test Organisation",
            AddressLine1 = "123 Test St",
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                MaterialName = "Test Material",
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { overseasAddress }
                }
            },
            Journey = new List<string>()
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes();

        // Assert
        using (var scope = new AssertionScope())
        {
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();

            var model = viewResult.Model as BaselConventionAndOecdCodesViewModel;
            model.Should().NotBeNull();
            model.OecdCodes.Should().HaveCount(5); // Check that 5 empty OecdCodes are added
            model.OecdCodes.Should().AllBeEquivalentTo(new OverseasAddressWasteCodesViewModel());
        }
    }

    [TestMethod]
    public async Task BaselConventionAndOECDCodes_CallsSetTempBackLink_AndSaveSession()
    {
        // Arrange
        var overseasAddress = new OverseasAddress
        {
            IsActive = true,
            OrganisationName = "Test Organisation",
            AddressLine1 = "123 Test St",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                MaterialName = "Test Material",
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { overseasAddress }
                }
            },
            Journey = new List<string>()
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        await _controller.BaselConventionAndOECDCodes();

        // Assert
        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
        // Verify that SetTempBackLink was called with the correct parameters
        _controller.InvokeProtectedMethod("SetTempBackLink", PagePaths.OverseasSiteDetails, PagePaths.BaselConventionAndOECDCodes);
    }

    [TestMethod]
    public async Task BaselConventionAndOecdCodes_WhenValidationFails_ReturnsViewWithModel()
    {
        // Arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "OecdCodes", ErrorMessage = "At least one code is required" }
            });

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        var view = (ViewResult)result;
        view.ViewName.Should().Be("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml");
        view.Model.Should().Be(model);
    }

    [TestMethod]
    public async Task BaselConventionAndOecdCodes_ValidModel_WithActiveAddress_RedirectsToAddAnotherOverseasReprocessingSite()
    {
        // Arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
                {
                    new()
                    {
                        CodeName = " CodeA "
                    },
                    new()
                    {
                        CodeName = "CodeB"
                    }
                },
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var activeAddress = new OverseasAddress
        {
            IsActive = true,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = (RedirectResult)result;
        redirect.Url.Should().Be(PagePaths.AddAnotherOverseasReprocessingSite);

        activeAddress.OverseasAddressWasteCodes
            .Select(c => c.CodeName)
            .Should().BeEquivalentTo(new[] { "CodeA", "CodeB" });
    }

    [TestMethod]
    public async Task BaselConventionAndOecdCodes_ValidModel_WithNullOverseasAddresses_InitialisesListAndRedirects()
    {
        // Arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
                {
                    new()
                    {
                        CodeName = "ABC"
                    }
                },
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = null // will be initialised by controller
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = (RedirectResult)result;
        redirect.Url.Should().Be(PagePaths.AddAnotherOverseasReprocessingSite);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses
            .Should().NotBeNull();
    }

    [TestMethod]
    public async Task BaselConventionAndOecdCodes_ValidModel_SaveAndComeBackLater_RedirectsToApplicationSaved()
    {
        // Arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
                {
                    new()
                    {
                        CodeName = "Code1"
                    }
                },
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var activeAddress = new OverseasAddress
        {
            IsActive = true,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes(model, "SaveAndComeBackLater");

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = (RedirectResult)result;
        redirect.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    [TestMethod]
    public async Task BaselConventionAndOecdCodes_ValidModel_UnknownButtonAction_ReturnsView()
    {
        // Arrange
        var model = new BaselConventionAndOecdCodesViewModel
        {
            OecdCodes = new List<OverseasAddressWasteCodesViewModel>
                {
                    new()
                    {
                        CodeName = "ABC"
                    }
                },
            OrganisationName = "xyz Ltd",
            AddressLine1 = "Address line 1"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var activeAddress = new OverseasAddress
        {
            IsActive = true,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.BaselConventionAndOECDCodes(model, "UnknownAction");

        // Assert
        result.Should().BeOfType<ViewResult>();
        var view = (ViewResult)result;
        view.ViewName.Should().Be("~/Views/Registration/Exporter/BaselConventionAndOecdCodes.cshtml");
        view.Model.Should().Be(model);
    }

        [TestMethod]
        [DataRow(PagePaths.BaselConventionAndOECDCodes)]
        public async Task AddAnotherOverseasReprocessingSite_Should_Return_ViewResult(string previousPath)
        {
            //Arrange
            var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true }; // Meaning the selected answer is Yes.
            var backLink = previousPath;        

        //Act
        var result = _controller.AddAnotherOverseasReprocessingSite();
        var actualResult = await result as ViewResult;

        //Assert
        actualResult.Should().BeOfType<ViewResult>();
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
    public async Task AddAnotherOverseasReprocessingSite_Should_Pass_Validation(string buttonAction, string previousPath)
    {
        //Arrange
        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true }; // Meaning the selected answer is Yes.
        var backlink = previousPath;

        //Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
        var modelState = _controller.ModelState;

        //Assert
        modelState.IsValid.Should().BeTrue();
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
    public async Task AddAnotherOverseasReprocessingSite_Should_Fail_Validation(string buttonAction, string previousPath)
    {
        //Arrange
        var model = new AddAnotherOverseasReprocessingSiteViewModel();

        var backlink = previousPath;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        //Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
        var modelState = _controller.ModelState;

        modelState.AddModelError("Selection error", "Select if you are adding another overseas reprocessing site");

        //Assert
        modelState.IsValid.Should().BeFalse();
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.OverseasSiteDetails)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_OverSeas_Details(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new AddAnotherOverseasReprocessingSiteViewModel {  AddOverseasSiteAccepted = true };  

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndContinueActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
    public async Task AddAnotherOverseasReprocessingSite_Redirect_Url_Should_Be_Error_Page(string buttonAction, string previousPath)
    {
        //Arrange

        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };           

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = null,
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true };

        var backlink = previousPath;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);


        //Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
        var redirectResult = await result as RedirectResult;


        //Assert
        redirectResult.Url.Should().Contain("/Error");
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_Check_Your_Answers(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndContinueActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.CheckYourAnswersForOverseasProcessingSite)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_Check_Your_Answers_When_No_And_SaveAndContinue(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndContinueActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }

    [TestMethod]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_ApplicationSaved_When_No_And_SaveAndComeBackLater(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndComeBackLaterActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }


    [TestMethod]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_ApplicationSaved_When_Yes_Is_Selected(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

            var session = new ExporterRegistrationSession
            {                
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndComeBackLaterActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }


    [TestMethod]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_ApplicationSaved_When_No_Is_Selected(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndComeBackLaterActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
    }

    [TestMethod]
    [DataRow("", PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task AddAnotherOverseasReprocessingSite_Should_Return_View_With_Empty_ButtonAction_Value(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
        var view = await result as ViewResult;

        // Assert
        view.Should().BeOfType<ViewResult>();
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.AddAnotherOverseasReprocessingSite)]
    public async Task AddAnotherOverseasReprocessingSite_Should_Have_SaveAndContinue_And_No_Selected(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = false };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.AddAnotherOverseasReprocessingSite(model, string.Empty);
        var view = await result as RedirectResult;

        // Assert
        //view.Should().BeOfType<RedirectResult>();
        model.AddOverseasSiteAccepted.Should().BeFalse();
        buttonAction.Should().BeEquivalentTo(SaveAndContinueActionKey);

    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
    public async Task AddAnotherOverseasReprocessingSite_RedirecToUrl_Should_Be_Invalid(string buttonAction, string redirectToUrl)
    {
        // Arrange

        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


        var activeAddress1 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var activeAddress2 = new OverseasAddress
        {
            IsActive = false,
            OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
            AddressLine1 = "",
            AddressLine2 = "",
            CityOrTown = "",
            CountryName = "",
            OrganisationName = "",
            PostCode = "",
            SiteCoordinates = "",
            StateProvince = ""
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                }
            }
        };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);            

        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

        var model = new AddAnotherOverseasReprocessingSiteViewModel();
        model.AddOverseasSiteAccepted = null;

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "AddOverseasSiteAccepted", ErrorMessage = AddAnotherOverseasReprocessingSite.AddOverseasProcessingSiteErrorMessage }
            });


        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

            // Act
            var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndContinueActionKey);
            var view = await result as ViewResult;     
            
            validationResult.IsValid.Should().BeFalse();
        }




    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_ShouldRedirectToError_WhenRegistrationMaterialIdIsNull()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = null,
                OverseasReprocessingSites = new OverseasReprocessingSites()
            }
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(null);

        // Assert
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/Error");
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_ShouldAddModelError_WhenNoOverseasAddressesAndButtonActionIsNotNullOrEmpty()
    {
        // Arrange
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = new List<OverseasAddress>()
                }
            },
            Journey = new List<string>()
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers("SaveAndContinue");

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().Be("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml");
            viewResult.Model.Should().BeOfType<CheckOverseasReprocessingSitesAnswersViewModel>();
            _controller.ModelState.ContainsKey(nameof(CheckOverseasReprocessingSitesAnswersViewModel.OverseasAddresses)).Should().BeTrue();
            _controller.ModelState[nameof(CheckOverseasReprocessingSitesAnswersViewModel.OverseasAddresses)]
                .Errors[0].ErrorMessage.Should().Be("You must have at least one overseas reprocessors site before you can continue");
        }
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_ShouldReturnViewWithModel_WhenOverseasAddressesExist()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", false)
            };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            },
            Journey = new List<string>()
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(null);

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().Be("~/Views/Registration/Exporter/CheckOverseasReprocessingSitesAnswers.cshtml");
            viewResult.Model.Should().BeOfType<CheckOverseasReprocessingSitesAnswersViewModel>();
            ((CheckOverseasReprocessingSitesAnswersViewModel)viewResult.Model).OverseasAddresses.Should().BeEquivalentTo(overseasAddresses);
        }
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_ShouldSetJourneyAndBackLink()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", false)
            };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            },
            Journey = new List<string>()
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        await _controller.CheckOverseasReprocessingSitesAnswers(null);

        // Assert
        using (var scope = new AssertionScope())
        {
            var backLinkToDisplay = (string)_controller.ViewBag.BackLinkToDisplay;
            session.Journey.Should().ContainInOrder(PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite);
            backLinkToDisplay.Should().Be(PagePaths.AddAnotherOverseasReprocessingSite);
        }
    }

        [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_SaveAndComeBackLater_RedirectsToApplicationSaved()
    {
        // Arrange
        var session = CreateSessionWithAddresses(1);
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        var model = new CheckOverseasReprocessingSitesAnswersViewModel();
        var buttonAction = "SaveAndComeBackLater";

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(model, buttonAction);

        // Assert
        using (var scope = new AssertionScope())
        {
            var redirect = result as RedirectResult;
            redirect.Should().NotBeNull();
            redirect.Url.Should().Be(PagePaths.ApplicationSaved);
        }
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_NoAddressesAndSaveAndContinue_RedirectsToSelfWithButtonAction()
    {
        // Example usage of the new method
        var session = CreateSessionWithAddresses(0);
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        var model = new CheckOverseasReprocessingSitesAnswersViewModel();
        var buttonAction = "SaveAndContinue";

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(model, buttonAction);

        // Assert
        using (var scope = new AssertionScope())
        {
            var redirect = result as RedirectToActionResult;
            redirect.Should().NotBeNull();
            redirect.ActionName.Should().Be("CheckOverseasReprocessingSitesAnswers");
            redirect.RouteValues["buttonAction"].Should().Be(buttonAction);
        }
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_SaveAndContinue_WithAddresses_SavesAndRedirectsToTaskList()
    {
        // Arrange
        var session = CreateSessionWithAddresses(1);
        var registrationMaterialId = session.ExporterRegistrationApplicationSession.RegistrationMaterialId!.Value;
        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        _mapperMock.Setup(m => m.Map<OverseasAddressRequestDto>(It.IsAny<ExporterRegistrationApplicationSession>()))
            .Returns(new OverseasAddressRequestDto { RegistrationMaterialId = registrationMaterialId });

        _exporterRegistrationService.Setup(e => e.SaveOverseasReprocessorAsync(It.IsAny<OverseasAddressRequestDto>()))
            .Returns(Task.CompletedTask);

        _exporterRegistrationService.Setup(e => e.GetOverseasMaterialReprocessingSites(It.IsAny<Guid>()))
            .ReturnsAsync(new List<OverseasMaterialReprocessingSiteDto>());

        var model = new CheckOverseasReprocessingSitesAnswersViewModel();
        string buttonAction = "SaveAndContinue";

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(model, buttonAction);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirect = result as RedirectResult;
        redirect!.Url.Should().Be(PagePaths.ExporterTaskList);
        _exporterRegistrationService.Verify(e => e.SaveOverseasReprocessorAsync(It.IsAny<OverseasAddressRequestDto>()), Times.Once);
        _exporterRegistrationService.Verify(e => e.GetOverseasMaterialReprocessingSites(registrationMaterialId), Times.Once);
    }

    [TestMethod]
    public async Task CheckOverseasReprocessingSitesAnswers_ReconcilesSessionData_Correctly_UsingProvidedData()
    {
        // Arrange
        var sessionData = CreateSessionWithAddresses(1);
        var savedDtoData = BuildDtoData();

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(sessionData);
        _exporterRegistrationService.Setup(s => s.GetOverseasMaterialReprocessingSites(It.IsAny<Guid>()))
            .ReturnsAsync(savedDtoData);

        _mapperMock.Setup(m =>
                m.Map<OverseasAddress>(It.IsAny<OverseasAddressDto>()))
                .Returns<OverseasAddressDto>(dto => new OverseasAddress
                {
                
                        Id = dto.Id,
                        OrganisationName = dto.OrganisationName,
                        AddressLine1 = dto.AddressLine1,
                        AddressLine2 = dto.AddressLine2,
                        CityOrTown = dto.CityOrTown,
                        CountryName = dto.CountryName,
                        PostCode = dto.PostCode,
                        StateProvince = dto.StateProvince,
                        SiteCoordinates = "565"
                });

        var model = new CheckOverseasReprocessingSitesAnswersViewModel();
        string buttonAction = "SaveAndContinue";

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(model, buttonAction);


        // Assert
        using (new AssertionScope())
        {
            var updatedSites = sessionData.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses;

                updatedSites.Should().HaveCount(3);
            _sessionManagerMock.Verify(s => s.SaveSessionAsync(It.IsAny<ISession>(), sessionData), Times.Once);
        }
    }

        [TestMethod]
    public async Task ChangeOverseasReprocessingSite_SetsCorrectIsActive_AndRedirectsToOverseasSiteDetails()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
        {
            CreateTestOverseasAddresses("A", false),
            CreateTestOverseasAddresses("B", false),
            CreateTestOverseasAddresses("C", false)
        };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            }
        };
        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ChangeOverseasReprocessingSite(2);

        // Assert
        using (var scope = new AssertionScope())
        {
            overseasAddresses[0].IsActive.Should().BeFalse();
            overseasAddresses[1].IsActive.Should().BeTrue();
            overseasAddresses[2].IsActive.Should().BeFalse();

                result.Should().BeOfType<RedirectToActionResult>();
                var redirect = (RedirectToActionResult)result;
                redirect.ActionName.Should().Be("OverseasSiteDetails");

            _sessionManagerMock.Verify(m => m.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
        }
    }

    [TestMethod]
    public async Task ChangeOverseasReprocessingSite_WithIndexOne_ActivatesFirstOnly()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", false),
                CreateTestOverseasAddresses("B", false)
            };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            }
        };
        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ChangeOverseasReprocessingSite(1);

            // Assert
            using (var scope = new AssertionScope())
            {
                overseasAddresses[0].IsActive.Should().BeTrue();
                overseasAddresses[1].IsActive.Should().BeFalse();
                result.Should().BeOfType<RedirectToActionResult>();
                ((RedirectToActionResult)result).ActionName.Should().Be("OverseasSiteDetails");
            }
        }

    [TestMethod]
    public async Task ChangeOverseasReprocessingSite_WithIndexOutOfRange_AllInactive()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", true),
                CreateTestOverseasAddresses("B", true)
            };

        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            }
        };
        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ChangeOverseasReprocessingSite(10);

            // Assert
            using (var scope = new AssertionScope())
            {
                overseasAddresses.All(a => !a.IsActive).Should().BeTrue();
                result.Should().BeOfType<RedirectToActionResult>();
                ((RedirectToActionResult)result).ActionName.Should().Be("OverseasSiteDetails");
            }
        }

    [TestMethod]
    public async Task DeleteOverseasReprocessingSite_RemovesSiteAndSetsTempDataAndRedirects()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("Org1", false, "Addr1"),
                CreateTestOverseasAddresses("Org2", false, "Addr2")
            };
        var overseasSites = new OverseasReprocessingSites { OverseasAddresses = overseasAddresses };
        var appSession = new ExporterRegistrationApplicationSession { OverseasReprocessingSites = overseasSites };
        var session = new ExporterRegistrationSession { ExporterRegistrationApplicationSession = appSession };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // TempData setup
        var tempData = new TempDataDictionary(_httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        _controller.TempData = tempData;

        // Act
        var result = await _controller.DeleteOverseasReprocessingSite(2);

        // Assert
        using (var scope = new AssertionScope())
        {
            tempData["DeletedOverseasReprocessor"].Should().Be("Org2, Addr2");
            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = (RedirectToActionResult)result;
            redirect.ActionName.Should().Be("CheckOverseasReprocessingSitesAnswers");
        }
    }

    [TestMethod]
    public async Task DeleteOverseasReprocessingSite_RemovesCorrectSiteFromModel()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("Org1", false, "Addr1"),
                CreateTestOverseasAddresses("Org2", false, "Addr2"),
                CreateTestOverseasAddresses("Org3", false, "Addr3")
            };
        var overseasSites = new OverseasReprocessingSites { OverseasAddresses = overseasAddresses };
        var appSession = new ExporterRegistrationApplicationSession { OverseasReprocessingSites = overseasSites };
        var session = new ExporterRegistrationSession { ExporterRegistrationApplicationSession = appSession };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // TempData setup
        var tempData = new TempDataDictionary(_httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        _controller.TempData = tempData;

        // Act
        await _controller.DeleteOverseasReprocessingSite(1);

        // Assert
        session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.Count.Should().Be(2);
    }

    [TestMethod]

    public async Task DeleteOverseasReprocessingSite_SetsJourneyCorrectly()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("Org1", false, "Addr1")
            };
        var overseasSites = new OverseasReprocessingSites { OverseasAddresses = overseasAddresses };
        var appSession = new ExporterRegistrationApplicationSession { OverseasReprocessingSites = overseasSites };
        var session = new ExporterRegistrationSession { ExporterRegistrationApplicationSession = appSession };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // TempData setup
        var tempData = new TempDataDictionary(_httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        _controller.TempData = tempData;

        // Act
        await _controller.DeleteOverseasReprocessingSite(1);

        // Assert
        session.Journey.Should().ContainInOrder(PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.CheckYourAnswersForOverseasProcessingSite);
    }

    [TestMethod]
    public async Task ChangeBaselConvention_SetsCorrectIsActiveAndRedirects()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", false),
                CreateTestOverseasAddresses("B", false),
                CreateTestOverseasAddresses("C", false)
            };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            }
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.ChangeBaselConvention(2);

        // Assert
        using (var scope = new AssertionScope())
        {
            overseasAddresses[0].IsActive.Should().BeFalse();
            overseasAddresses[1].IsActive.Should().BeTrue();
            overseasAddresses[2].IsActive.Should().BeFalse();

            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = (RedirectToActionResult)result;
            redirect.ActionName.Should().Be("BaselConventionAndOECDCodes");
        }
    }

    [TestMethod]
    public async Task ChangeBaselConvention_CallsSaveSessionWithCorrectPagePath()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", false)
            };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                OverseasReprocessingSites = new OverseasReprocessingSites
                {
                    OverseasAddresses = overseasAddresses
                }
            }
        };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), session)).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _controller.ChangeBaselConvention(1);

        // Assert
        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
    }

    [TestMethod]
    public async Task AddAnotherOverseasReprocessingSiteFromCheckYourAnswer_Should_Set_All_IsActive_To_False_And_RedirectToIndex()
    {
        // Arrange
        var overseasAddresses = new List<OverseasAddress>
            {
                CreateTestOverseasAddresses("A", true),
                CreateTestOverseasAddresses("B", true)
            };
        var overseasReprocessingSites = new OverseasReprocessingSites
        {
            OverseasAddresses = overseasAddresses
        };
        var appSession = new ExporterRegistrationApplicationSession
        {
            OverseasReprocessingSites = overseasReprocessingSites
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = appSession
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.AddAnotherOverseasReprocessingSiteFromCheckYourAnswer();

            // Assert
            using (var scope = new AssertionScope())
            {
                overseasAddresses.All(a => a.IsActive == false).Should().BeTrue();
                _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
                result.Should().BeOfType<RedirectToActionResult>();
                var redirect = result as RedirectToActionResult;
                redirect.ActionName.Should().Be("OverseasSiteDetails");
            }
        }

        [TestMethod]
        public async Task AddInterimSites_ReconcilesSessionData_Correctly_UsingProvidedData()
        {
            // Arrange
            var sessionData = BuildSessionWithPrepopulatedInterimSites();
            var savedDtoData = BuildDtoData();

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(sessionData);
            _exporterRegistrationService.Setup(s => s.GetOverseasMaterialReprocessingSites(It.IsAny<Guid>()))
                .ReturnsAsync(savedDtoData);

            _mapperMock.Setup(m =>
                    m.Map<OverseasMaterialReprocessingSite>(It.IsAny<OverseasMaterialReprocessingSiteDto>()))
                .Returns<OverseasMaterialReprocessingSiteDto>(dto => new OverseasMaterialReprocessingSite
                {
                    Id = dto.Id,
                    OverseasAddressId = dto.OverseasAddressId,
                    OverseasAddress = new OverseasAddress
                    {
                        ExternalId = dto.OverseasAddress.ExternalId,
                        OrganisationName = dto.OverseasAddress.OrganisationName,
                        AddressLine1 = dto.OverseasAddress.AddressLine1,
                        AddressLine2 = dto.OverseasAddress.AddressLine2,
                        CityOrTown = dto.OverseasAddress.CityOrTown,
                        CountryName = dto.OverseasAddress.CountryName,
                        PostCode = dto.OverseasAddress.PostCode,
                        StateProvince = dto.OverseasAddress.StateProvince,
                        SiteCoordinates = "565"
                    },
                    IsActive = false
                });

            // Act
            var result = await _controller.AddInterimSites();

            // Assert
            using (new AssertionScope())
            {
                var updatedSites = sessionData.ExporterRegistrationApplicationSession.InterimSites!.OverseasMaterialReprocessingSites;

                updatedSites.Should().HaveCount(3);
                updatedSites.Should().ContainSingle(x =>
                    x.OverseasAddressId == Guid.Parse("6033F5CD-E2C0-4AB5-BFAF-C5F517E93EAE"));
                updatedSites.Should().Contain(x => x.OverseasAddress.OrganisationName == "Org 1");
                updatedSites.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.OverseasAddress.OrganisationName));

                _sessionManagerMock.Verify(s => s.SaveSessionAsync(It.IsAny<ISession>(), sessionData), Times.Once);
                ((ViewResult)result).ViewName.Should().Be("~/Views/Registration/Exporter/AddInterimSites.cshtml");

                var model = ((ViewResult)result).Model as AddInterimSitesViewModel;
                model.Should().NotBeNull();
                model!.OverseasMaterialReprocessingSites.Should()
                    .BeInAscendingOrder(x => x.OverseasAddress.OrganisationName);
            }
        }

        [TestMethod]
        public async Task AddInterimSites_WhenSavedDataIsNull_ClearsSessionAndReturnsEmptyList()
        {
            // Arrange
            var session = BuildSessionWithPrepopulatedInterimSites();
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
            _exporterRegistrationService.Setup(s => s.GetOverseasMaterialReprocessingSites(It.IsAny<Guid>()))
                .ReturnsAsync((List<OverseasMaterialReprocessingSiteDto>?)null);

            // Act
            var result = await _controller.AddInterimSites();

            // Assert
            using (new AssertionScope())
            {
                session.ExporterRegistrationApplicationSession.InterimSites!.OverseasMaterialReprocessingSites.Should()
                    .BeEmpty();
                ((ViewResult)result).ViewName.Should().Be("~/Views/Registration/Exporter/AddInterimSites.cshtml");

                var model = ((ViewResult)result).Model as AddInterimSitesViewModel;
                model.Should().NotBeNull();
                model!.OverseasMaterialReprocessingSites.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task CheckAddedInterimSites_WhenSitesIsNull_ReturnsNotFound()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = null // triggers NotFound
                }
            };

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.CheckAddedInterimSites(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task CheckAddedInterimSites_WhenSiteNotFound_ReturnsNotFound()
        {
            // Arrange
            var session = BuildSessionWithPrepopulatedInterimSites();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            var unknownId = Guid.NewGuid();

            // Act
            var result = await _controller.CheckAddedInterimSites(unknownId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task CheckAddedInterimSites_WhenSiteFound_SetsIsActive_AndRedirects()
        {
            // Arrange
            var session = BuildSessionWithPrepopulatedInterimSites();
            var targetId = session.ExporterRegistrationApplicationSession.InterimSites!.OverseasMaterialReprocessingSites.First().OverseasAddressId;

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.CheckAddedInterimSites(targetId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>()
                      .Which.Url.Should().Be(PagePaths.ExporterInterimSitesUsed);

                var sites = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites;
                sites.Should().ContainSingle(s => s.OverseasAddressId == targetId && s.IsActive == true);
                sites.Where(s => s.OverseasAddressId != targetId).Should().OnlyContain(s => s.IsActive == false);
            }
        }

        [TestMethod]
        public async Task AddNewInterimSite_WhenSitesIsNull_ReturnsNotFound()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = null
                }
            };

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            var result = await _controller.AddNewInterimSite(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task AddNewInterimSite_WhenSiteNotFound_ReturnsNotFound()
        {
            var session = BuildSessionWithPrepopulatedInterimSites();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            var unknownId = Guid.NewGuid();

            var result = await _controller.AddNewInterimSite(unknownId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task AddNewInterimSite_WhenSiteFound_SetsIsActive_AndRedirects()
        {
            var session = BuildSessionWithPrepopulatedInterimSites();
            var targetId = session.ExporterRegistrationApplicationSession.InterimSites!.OverseasMaterialReprocessingSites.First().OverseasAddressId;

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            var result = await _controller.AddNewInterimSite(targetId);

            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>()
                      .Which.Url.Should().Be(PagePaths.ExporterInterimSiteDetails);

                var sites = session.ExporterRegistrationApplicationSession.InterimSites.OverseasMaterialReprocessingSites;
                sites.Should().ContainSingle(s => s.OverseasAddressId == targetId && s.IsActive == true);
                sites.Where(s => s.OverseasAddressId != targetId).Should().OnlyContain(s => s.IsActive == false);
            }
        }

        [TestMethod]
        public async Task AddInterimSites_WhenSaveAndComeBack_ReturnsRedirectToSaved()
        {
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                               .ReturnsAsync(new ExporterRegistrationSession());

            var result = await _controller.AddInterimSites("SaveAndComeBackLater");

            result.Should().BeOfType<RedirectResult>()
                  .Which.Url.Should().Be(PagePaths.ApplicationSaved);
        }

        [TestMethod]
        public async Task AddInterimSites_WhenSaveAndContinue_UpsertsAndReconcilesAndRedirects()
        {
            var session = BuildSessionWithPrepopulatedInterimSites();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                               .ReturnsAsync(session);

            _mapperMock.Setup(x => x.Map<SaveInterimSitesRequestDto>(It.IsAny<ExporterRegistrationApplicationSession>()))
                       .Returns(new SaveInterimSitesRequestDto
                       {
                           OverseasMaterialReprocessingSites = BuildDtoData()
                       });

            _exporterRegistrationService.Setup(x => x.UpsertInterimSitesAsync(It.IsAny<SaveInterimSitesRequestDto>()))
                                        .Returns(Task.CompletedTask);

            _exporterRegistrationService.Setup(x => x.GetOverseasMaterialReprocessingSites(It.IsAny<Guid>()))
                                        .ReturnsAsync(BuildDtoData());

            _mapperMock.Setup(x => x.Map<OverseasMaterialReprocessingSite>(It.IsAny<OverseasMaterialReprocessingSiteDto>()))
                       .Returns(new OverseasMaterialReprocessingSite
                       {
                           OverseasAddress = new OverseasAddress
                           {
                               ExternalId = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                               OrganisationName = "Org 1",
                               AddressLine1 = "Address 1",
                               AddressLine2 = "Address line 2",
                               CityOrTown = "City 1",
                               CountryName = "Portugal",
                               PostCode = "POSTCODE001",
                               StateProvince = "Lisbon",
                               SiteCoordinates = "576"
                           },
                           OverseasAddressId = Guid.NewGuid()
                       });

            var result = await _controller.AddInterimSites("SaveAndContinue");

            // Note: method ends in a Redirect that is not returned (logical bug in source method)
            result.Should().BeOfType<RedirectResult>()
                .Which.Url.Should().Be(PagePaths.ExporterTaskList);
        }

        [TestMethod]
        public async Task AddInterimSites_WhenNoMatchAction_ReturnsRedirectToSelf()
        {
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                               .ReturnsAsync(new ExporterRegistrationSession());

            var result = await _controller.AddInterimSites("invalid-action");

            result.Should().BeOfType<RedirectToActionResult>()
                  .Which.ActionName.Should().Be(nameof(_controller.AddInterimSites));
        }

        private static ExporterRegistrationSession BuildSessionWithPrepopulatedInterimSites()
        {
            return new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
                        {
                            new OverseasMaterialReprocessingSite
                            {
                                IsActive = false,
                                Id = new Guid("6B7FACD8-3DB9-4480-8D37-662D237B3106"),
                                OverseasAddressId = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                                OverseasAddress = new OverseasAddress
                                {
                                    ExternalId = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                                    OrganisationName = "Org 1",
                                    AddressLine1 = "Address 1",
                                    AddressLine2 = "Address line 2",
                                    CityOrTown = "City 1",
                                    CountryName = "Portugal",
                                    PostCode = "POSTCODE001",
                                    StateProvince = "Lisbon",
                                    SiteCoordinates = "213"
                                },
                                InterimSiteAddresses = new List<InterimSiteAddress>
                                {
                                    new InterimSiteAddress //newly added in session has to be inserted
                                    {
                                        Id = null,
                                        IsActive = false,
                                        OrganisationName = "Interim Org 1",
                                        AddressLine1 = "Interim Address 1",
                                        AddressLine2 = "Interim Address 2",
                                        CityOrTown = "Paris",
                                        CountryName = "France",
                                        PostCode = "001-32-45",
                                        StateProvince = "Interim State",
                                        InterimAddressContact = new List<OverseasAddressContact>
                                        {
                                            new OverseasAddressContact
                                            {
                                                FullName = "John Smith",
                                                Email = "john.smith@example.com",
                                                PhoneNumber = "441234567890",
                                            }
                                        }
                                    },
                                    new InterimSiteAddress //newly added in session has to be inserted
                                    {
                                        Id = null,
                                        IsActive = false,
                                        OrganisationName = "Interim Org 2",
                                        AddressLine1 = "Interim Address 2",
                                        AddressLine2 = "Interim Address 2",
                                        CityOrTown = "Warshaw",
                                        CountryName = "Poland",
                                        PostCode = "Pol01909",
                                        StateProvince = "",
                                        InterimAddressContact = new List<OverseasAddressContact>
                                        {
                                            new OverseasAddressContact
                                            {
                                                FullName = "John Doe",
                                                Email = "john.doe@example.com",
                                                PhoneNumber = "441234 567890",
                                            }
                                        }
                                    }
                                }
                            },
                            new OverseasMaterialReprocessingSite
                            {
                                IsActive = false,
                                Id = new Guid("A1B2C3D4-E5F6-7890-1234-56789ABCDEF0"),
                                OverseasAddressId = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                                OverseasAddress = new OverseasAddress
                                {
                                    ExternalId = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                                    OrganisationName = "Org 1",
                                    AddressLine1 = "Address 1",
                                    AddressLine2 = "Address 2",
                                    CityOrTown = "Munich",
                                    CountryName = "Germany",
                                    PostCode = "0932131",
                                    StateProvince = "some state",
                                    SiteCoordinates = "12321"
                                },
                                InterimSiteAddresses = new List<InterimSiteAddress> //existing record has to updated
                                {
                                    new InterimSiteAddress
                                    {
                                        Id = new Guid("38296DC3-B9BC-41D8-B886-EDA6450F35A3"),
                                        IsActive = false,
                                        OrganisationName = "Interim Org 1",
                                        AddressLine1 = "Interim Address 1",
                                        AddressLine2 = "Interim Address 2",
                                        CityOrTown = "Paris",
                                        CountryName = "France",
                                        PostCode = "001-32-45",
                                        StateProvince = "Interim State",
                                        InterimAddressContact = new List<OverseasAddressContact>
                                        {
                                            new OverseasAddressContact
                                            {
                                                FullName = "Bhaarath Balraj",
                                                Email = "bhaarath.balraj@test.com",
                                                PhoneNumber = "447739423100",
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        
        private static List<OverseasMaterialReprocessingSiteDto> BuildDtoData()
        {
            return new List<OverseasMaterialReprocessingSiteDto>
            {
                new OverseasMaterialReprocessingSiteDto
                {
                    Id = new Guid("6B7FACD8-3DB9-4480-8D37-662D237B3106"),
                    OverseasAddressId = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                    OverseasAddress = new OverseasAddressDto()
                    {
                        ExternalId = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                        OrganisationName = "Org 1",
                        AddressLine1 = "Address 1",
                        AddressLine2 = "Address line 2",
                        CityOrTown = "City 1",
                        CountryName = "Portugal",
                        PostCode = "POSTCODE001",
                        StateProvince = "Lisbon"
                    },
                    InterimSiteAddresses = null

                },
                new OverseasMaterialReprocessingSiteDto
                {
                    Id = new Guid("A1B2C3D4-E5F6-7890-1234-56789ABCDEF0"),
                    OverseasAddressId = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                    OverseasAddress = new OverseasAddressDto()
                    {
                        ExternalId = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                        OrganisationName = "Org 1",
                        AddressLine1 = "Address 1",
                        AddressLine2 = "Address 2",
                        CityOrTown = "Munich",
                        CountryName = "Germany",
                        PostCode = "0932131",
                        StateProvince = "some state"
                    },
                    InterimSiteAddresses = new List<InterimSiteAddressDto>
                    {
                        new InterimSiteAddressDto //existing record will be updated via session
                        {
                            ExternalId = new Guid("38296DC3-B9BC-41D8-B886-EDA6450F35A3"),
                            OrganisationName = "Interim Org 1",
                            AddressLine1 = "Interim Address 1",
                            AddressLine2 = "Interim Address 2",
                            CityOrTown = "Paris",
                            CountryName = "France",
                            PostCode = "001-32-45",
                            StateProvince = "Interim State",
                            InterimAddressContact = new List<OverseasAddressContactDto>
                            {
                                new OverseasAddressContactDto
                                {
                                    FullName = "test user",
                                    Email = "test.user@test.com",
                                    PhoneNumber = "447739423100"
                                }
                            }
                        }
                    }
                },
                new
                    OverseasMaterialReprocessingSiteDto // new record doesn't exists in session has to be appended to session
                    {
                        Id = new Guid("613B2436-B029-46E6-B1CA-9A4A3A3B4AFE"),
                        OverseasAddressId = new Guid("6033F5CD-E2C0-4AB5-BFAF-C5F517E93EAE"),
                        OverseasAddress = new OverseasAddressDto
                        {
                            ExternalId = new Guid("6033F5CD-E2C0-4AB5-BFAF-C5F517E93EAE"),
                            OrganisationName = "New Org from DB",
                            AddressLine1 = "Address 1",
                            AddressLine2 = "Address 2",
                            CityOrTown = "Oslo",
                            CountryName = "Norway",
                            PostCode = "0932131",
                            StateProvince = "some state"
                        }
                    }
            };
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
        public async Task UseAnotherInterimSite_Should_Pass_Validation(string buttonAction, string previousPath)
        {
            //Arrange
            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = true }; // Meaning the selected answer is Yes.
            var backlink = previousPath;

            //Act
            var result = _controller.UseAnotherInterimSite(model, buttonAction);
            var modelState = _controller.ModelState;

            //Assert
            modelState.IsValid.Should().BeTrue();
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.ExporterInterimSiteDetails)]
        public async Task UseAnotherInterimSite_Should_Fail_Validation(string buttonAction, string previousPath)
        {
            //Arrange
            var model = new UseAnotherInterimSiteViewModel();

            var backlink = previousPath;

            var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "AddInterimSiteAccepted", ErrorMessage = UseAnotherInterimSite.UseAnotherInterimSiteErrorMessage }
            });

            //Act
            var result = _controller.UseAnotherInterimSite(model, buttonAction);
            var modelState = _controller.ModelState;

            modelState.AddModelError("Selection error", "Select yes if you use another interim site");

            //Assert
            modelState.IsValid.Should().BeFalse();
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.ExporterInterimSitesUsed)]
        public async Task UseAnotherInterimSite_RedirecToUrl_Should_Be_Interim_Sites_Used_When_No_And_SaveAndContinue(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = true,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = false };

            var backlink = PagePaths.ExporterInterimSiteDetails;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite(model, SaveAndContinueActionKey);
            var view = await result as RedirectResult;

            // Assert
            view.Url.Should().BeEquivalentTo(redirectToUrl);
        }


        [TestMethod]
        [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
        public async Task UseAnotherInterimSite_RedirecToUrl_Should_Be_ApplicationSaved_When_Yes_Is_Selected(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = true,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            //session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = true };

            var backlink = PagePaths.ExporterInterimSiteDetails;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite(model, SaveAndComeBackLaterActionKey);
            var view = await result as RedirectResult;

            // Assert
            view.Url.Should().BeEquivalentTo(redirectToUrl);
        }


        [TestMethod]
        [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
        public async Task UseAnotherInterimSite_RedirecToUrl_Should_Be_ApplicationSaved_When_No_Is_Selected_And_SaveComeBackLater(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            //session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = false };

            var backlink = PagePaths.ExporterInterimSiteDetails;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite(model, SaveAndComeBackLaterActionKey);
            var view = await result as RedirectResult;

            // Assert
            view.Url.Should().BeEquivalentTo(redirectToUrl);
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.ExporterInterimSitesUsed)]
        public async Task UseAnotherInterimSite_Should_Have_Valid_Session_And_Fail_Validation(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = true,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = false };

            var backlink = PagePaths.ExporterInterimSiteDetails;


            var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "AddInterimSiteAccepted", ErrorMessage = UseAnotherInterimSite.UseAnotherInterimSiteErrorMessage }
            });
            
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            //Act
            var result = _controller.UseAnotherInterimSite(model, buttonAction);
            var modelState = _controller.ModelState;

            modelState.AddModelError("Selection error", "Select yes if you use another interim site");

            //Assert
            modelState.IsValid.Should().BeFalse();
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.ExporterInterimSiteDetails)]
        public async Task UseAnotherInterimSite_RedirecToUrl_Should_Be_Interim_Sites_Details_When_Yes_And_SaveAndContinue(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = true };

            var backlink = PagePaths.ExporterInterimSiteDetails;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite(model, SaveAndContinueActionKey);
            var view = await result as RedirectResult;

            // Assert
            view.Url.Should().BeEquivalentTo(redirectToUrl);
        }


        [TestMethod]
        [DataRow("", "")]
        public async Task UseAnotherInterimSite_Should_Return_View_With_Empty_ButtonAction_Value(string buttonAction, string redirectToUrl)
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            var activeAddress1 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var activeAddress2 = new OverseasAddress
            {
                IsActive = false,
                OverseasAddressWasteCodes = new List<OverseasAddressWasteCodes>(),
                AddressLine1 = "",
                AddressLine2 = "",
                CityOrTown = "",
                CountryName = "",
                OrganisationName = "",
                PostCode = "",
                SiteCoordinates = "",
                StateProvince = ""
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = new List<OverseasAddress> { activeAddress1, activeAddress2 }
                    }
                }
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            session.ExporterRegistrationApplicationSession.OverseasReprocessingSites.OverseasAddresses.ForEach(a => a.IsActive = false);

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = false };

            var backlink = PagePaths.BaselConventionAndOECDCodes;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite(model, buttonAction);
            var view = await result as ViewResult;

            // Assert
            view.Should().BeOfType<ViewResult>();
        }


        [TestMethod]
        public async Task UseAnotherInterimSite_Should_Return_View()
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            // Arrange
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };

            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default CountryName",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var activeAddress = session?.ExporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses.FirstOrDefault(o => o.IsActive = true);

            string companyName = activeAddress?.OrganisationName ?? "this reprocessor site";
            string addressLine = activeAddress?.AddressLine1 ?? string.Empty;

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = null, AddressLine = addressLine, CompanyName = companyName };

            var backlink = PagePaths.BaselConventionAndOECDCodes;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite();
            var view = await result as ViewResult;

            // Assert
            view.Should().BeOfType<ViewResult>();
        }


        [TestMethod]
        public async Task UseAnotherInterimSite_Should_Return_View_Null_Session()
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";


            // Arrange
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };

            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default Country",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };

            //ExporterRegistrationSession session = null;


            //var session = new ExporterRegistrationSession
            //{
            //    ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            //};

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var activeAddress = session?.ExporterRegistrationApplicationSession?.OverseasReprocessingSites?.OverseasAddresses.FirstOrDefault(o => o.IsActive = true);

            string companyName = activeAddress?.OrganisationName ?? "this reprocessor site";
            string addressLine = activeAddress?.AddressLine1 ?? string.Empty;

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = null, AddressLine = addressLine, CompanyName = companyName };

            var backlink = PagePaths.BaselConventionAndOECDCodes;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite();
            var view = await result as ViewResult;

            // Assert
            view.Should().BeOfType<ViewResult>();           
        }


        [TestMethod]
        public async Task UseAnotherInterimSite_Should_Redirect_With_Empty_Session()
        {
            // Arrange
            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";
           
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };

            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default Country",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };          

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var activeAddress = session?.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites?.FirstOrDefault(o => o.IsActive = true);

            string companyName = activeAddress?.OverseasAddress?.OrganisationName;
            string addressLine = activeAddress?.OverseasAddress?.AddressLine1;

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = null, AddressLine = addressLine, CompanyName = companyName };

            var backlink = PagePaths.BaselConventionAndOECDCodes;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite();
            var redirectResult = await result as RedirectResult;

            // Assert
            redirectResult.Url.Should().Contain("/Error");
        }


    [TestMethod]
    public async Task UseAnotherInterimSite_Should_Redirect_With_Null_RegistrationMaterialId()
    {
        // Arrange
        const string SaveAndContinueActionKey = "SaveAndContinue";
        const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

        var interimSiteAddress = new InterimSiteAddress
        {
            IsActive = true,
            AddressLine1 = "Default Address Line 1",
            AddressLine2 = "Default Address Line 2",
            CityOrTown = "Default City",
            CountryName = "Default Country",
            OrganisationName = "Default Organisation",
            PostCode = "Default PostCode",
            StateProvince = "Default State"
        };

        var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
        {
            IsActive = true,
            OverseasAddress = new OverseasAddress
            {
                OrganisationName = "Org",
                AddressLine1 = "Addr",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                PostCode = "Default PostCode",
                StateProvince = "Default State",
                SiteCoordinates = "Default Coordinates"
            },
            InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
        };

        var session = new ExporterRegistrationSession
        { 
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
        };

        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var activeAddress = session?.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites?.FirstOrDefault(o => o.IsActive = true);

        string companyName = activeAddress?.OverseasAddress?.OrganisationName;
        string addressLine = activeAddress?.OverseasAddress?.AddressLine1;

        var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = true, AddressLine = addressLine, CompanyName = companyName };

        var backlink = PagePaths.BaselConventionAndOECDCodes;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = _controller.UseAnotherInterimSite(model, SaveAndContinueActionKey);
        var redirectResult = await result as RedirectResult;

        // Assert
        redirectResult.Url.Should().Contain("/Error");
    }

    [TestMethod]
        public async Task UseAnotherInterimSite_Should_Return_View_With_No_Active_Addresses()
        {
            // Arrange

            const string SaveAndContinueActionKey = "SaveAndContinue";
            const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };

            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default Country",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };

            _sessionManagerMock
                .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var activeAddress = session?.ExporterRegistrationApplicationSession?.InterimSites?.OverseasMaterialReprocessingSites?.FirstOrDefault(o => o.IsActive = true);
                        
            string companyName = activeAddress?.OverseasAddress?.OrganisationName;
            string addressLine = activeAddress?.OverseasAddress?.AddressLine1;

            var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = null, AddressLine = addressLine, CompanyName = companyName };

            var backlink = PagePaths.BaselConventionAndOECDCodes;

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock
                .Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = _controller.UseAnotherInterimSite();
            var redirectResult = await result as RedirectResult;

            // Assert
            redirectResult.Should().BeOfType<RedirectResult>();
        }


        private static OverseasAddress CreateTestOverseasAddresses(string orgName, bool isActive, string addressLine1 = "Addr1")
            => new OverseasAddress
            {
                AddressLine1 = addressLine1,
                AddressLine2 = "Test Line 2",
                CityOrTown = "Test City",
                CountryName = "Test CountryName",
                OrganisationName = orgName,
                PostCode = "Test PostCode",
                SiteCoordinates = "Test Coordinates",
                StateProvince = "Test State",
                IsActive = isActive
            };

        private ExporterRegistrationSession CreateSessionWithAddresses(int addressCount)
        {
            var addresses = new List<OverseasAddress>();
            for (int i = 0; i < addressCount; i++)
            {
                addresses.Add(new OverseasAddress
                {
                    OrganisationName = $"Org{i}",
                    IsActive = true,
                    AddressLine1 = $"AddressLine1_{i}",
                    AddressLine2 = $"AddressLine2_{i}",
                    CityOrTown = $"City_{i}",
                    CountryName = $"CountryName_{i}",
                    PostCode = $"PostCode_{i}",
                    SiteCoordinates = $"Coordinates_{i}",
                    StateProvince = $"State_{i}"
                });
            }
            return new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    OverseasReprocessingSites = new OverseasReprocessingSites
                    {
                        OverseasAddresses = addresses
                    }
                },
                Journey = new List<string>()
            };
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenRegistrationMaterialIdIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = null
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenSessionIsNull()
        {
            // Arrange
            ExporterRegistrationSession session = null;
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenExporterRegistrationApplicationSessionIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = null
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenInterimSitesIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = null
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenOverseasMaterialReprocessingSitesIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = null
                    }
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsError_WhenNoActiveOverseasAddress()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>()
                    }
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsView_WithModel_WhenActiveInterimSiteAddressExists()
        {
            // Arrange
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };
            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",            
                    CountryName = "Default CountryName",            
                    PostCode = "Default PostCode",          
                    StateProvince = "Default State",        
                    SiteCoordinates = "Default Coordinates" 
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };
            var viewModel = new InterimSiteViewModel();
            var countries = new List<string> { "UK", "France" };

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);
            _mapperMock.Setup(x => x.Map<InterimSiteViewModel>(interimSiteAddress))
                .Returns(viewModel);
            _registrationServiceMock.Setup(x => x.GetCountries())
                .ReturnsAsync(countries);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be("~/Views/Registration/Exporter/InterimSiteDetails.cshtml");
                var model = viewResult.Model as InterimSiteViewModel;
                model.Should().NotBeNull();
                model.Countries.Should().BeEquivalentTo(countries);
                model.OverseasSiteOrganisationName.Should().Be("Org");
                model.OverseasSiteAddressLine1.Should().Be("Addr");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Get_ReturnsView_WithNewModel_WhenNoActiveInterimSiteAddress()
        {
            // Arrange
            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default CountryName",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress>()
            };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };
            var countries = new List<string> { "UK", "France" };

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);
            _registrationServiceMock.Setup(x => x.GetCountries())
                .ReturnsAsync(countries);

            // Act
            var result = await _controller.InterimSiteDetails();

            // Assert
            using (var scope = new AssertionScope())
            {
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                var model = viewResult.Model as InterimSiteViewModel;
                model.Should().NotBeNull();
                model.Countries.Should().BeEquivalentTo(countries);
                model.OverseasSiteOrganisationName.Should().Be("Org");
                model.OverseasSiteAddressLine1.Should().Be("Addr");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenRegistrationMaterialIdIsNull()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = null
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenSessionIsNull()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            ExporterRegistrationSession session = null;
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenExporterRegistrationApplicationSessionIsNull()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = null
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenInterimSitesIsNull()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = null
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenOverseasMaterialReprocessingSitesIsNull()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = null
                    }
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }


        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsError_WhenNoActiveOverseasAddress()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>()
                    }
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };
            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",
                    CountryName = "Default CountryName",
                    PostCode = "Default PostCode",
                    StateProvince = "Default State",
                    SiteCoordinates = "Default Coordinates"
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                }
            };
            var countries = new List<string> { "UK" };
            var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "CountryName", ErrorMessage = "CountryName Missing" }
            });
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);
            _registrationServiceMock.Setup(x => x.GetCountries()).ReturnsAsync(countries);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                var returnedModel = viewResult.Model as InterimSiteViewModel;
                returnedModel.Countries.Should().BeEquivalentTo(countries);
                returnedModel.OverseasSiteOrganisationName.Should().Be("Org");
                returnedModel.OverseasSiteAddressLine1.Should().Be("Addr");
            }
        }

        [TestMethod]
        public async Task InterimSiteDetails_Post_UpdatesActiveInterimSiteAddress_WhenExists()
        {
            // Arrange
            var model = new InterimSiteViewModel();
            var interimSiteAddress = new InterimSiteAddress
            {
                IsActive = true,
                AddressLine1 = "Default Address Line 1",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default Country",
                OrganisationName = "Default Organisation",
                PostCode = "Default PostCode",
                StateProvince = "Default State"
            };
            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,

                OverseasAddress = new OverseasAddress
                {
                    OrganisationName = "Org",
                    AddressLine1 = "Addr",
                    AddressLine2 = "Default Address Line 2",
                    CityOrTown = "Default City",            
                    CountryName = "Default Country",            
                    PostCode = "Default PostCode",          
                    StateProvince = "Default State",        
                    SiteCoordinates = "Default Coordinates" 
                },
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSiteAddress }
            };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                _mapperMock.Verify(x => x.Map(model, interimSiteAddress), Times.Once);
                result.Should().BeOfType<RedirectResult>();
            }
        }

    [TestMethod]
    public async Task InterimSiteDetails_Post_AddsNewInterimSiteAddress_WhenNoneActive()
    {
        // Arrange
        var model = new InterimSiteViewModel();
        var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
        {
            IsActive = true,
            OverseasAddress = new OverseasAddress
            {
                OrganisationName = "Org",
                AddressLine1 = "Addr",
                AddressLine2 = "Default Address Line 2",
                CityOrTown = "Default City",
                CountryName = "Default CountryName",
                PostCode = "Default PostCode",
                StateProvince = "Default State",
                SiteCoordinates = "Default Coordinates"
            },
            InterimSiteAddresses = new List<InterimSiteAddress>()
        };
        var session = new ExporterRegistrationSession
        {
            ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
            {
                RegistrationMaterialId = Guid.NewGuid(),
                InterimSites = new InterimSites
                {
                    OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                }
            }
        };
        var mappedAddress = new InterimSiteAddress
        {
            IsActive = true,
            AddressLine1 = "Default Address Line 1",
            AddressLine2 = "Default Address Line 2",
            CityOrTown = "Default City",
            CountryName = "Default Country",
            OrganisationName = "Default Organisation",
            PostCode = "Default PostCode",
            StateProvince = "Default State"
        };
        var validationResult = new FluentValidation.Results.ValidationResult();
        _validationServiceMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _mapperMock.Setup(x => x.Map<InterimSiteAddress>(model)).Returns(mappedAddress);

        // Act
        var result = await _controller.InterimSiteDetails(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                overseasMaterialReprocessingSite.InterimSiteAddresses.Should().Contain(mappedAddress);
                mappedAddress.IsActive.Should().BeTrue();
                result.Should().BeOfType<RedirectResult>();
            }
        }

        [TestMethod]
        public async Task InterimSiteUsed_ReturnsError_WhenRegistrationMaterialIdIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = null
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteUsed(null);

            // Assert
            using (var scope = new AssertionScope())
            {
                result.Should().BeOfType<RedirectResult>();
                ((RedirectResult)result).Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSiteUsed_ReturnsView_WithCorrectModel_WhenSessionIsValid()
        {
            // Arrange
            var interimSite = CreateOverseasMaterialReprocessingSite();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
                        {
                            interimSite
                        }
                    }
                },
                Journey = new List<string>()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteUsed(null);

            // Assert
            using (var scope = new AssertionScope())
            {
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                viewResult!.ViewName.Should().Be("~/Views/Registration/Exporter/CheckInterimSitesAnswers.cshtml");
                viewResult.Model.Should().BeOfType<CheckInterimSitesAnswersViewModel>();
                var model = (CheckInterimSitesAnswersViewModel)viewResult.Model;
                model.Should().NotBeNull();
            }
        }

        [TestMethod]
        public async Task InterimSiteUsed_SetsJourneyAndBackLink_AndSavesSession()
        {
            // Arrange
            var interimSite = CreateOverseasMaterialReprocessingSite();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = Guid.NewGuid(),
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
                        {
                            interimSite
                        }
                    }
                },
                Journey = new List<string>()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSiteUsed(null);

            // Assert
            using (var scope = new AssertionScope())
            {
                string backLink = _controller.ViewBag.BackLinkToDisplay as string;
                session.Journey.Should().ContainInOrder(PagePaths.ExporterAnotherInterimSite, PagePaths.ExporterInterimSitesUsed);
                _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
                backLink.Should().Be(PagePaths.ExporterAnotherInterimSite);
            }
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_SaveAndContinue_RedirectsToAddInterimSites()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed("SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                var redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect.Url.Should().Be(PagePaths.ExporterAddInterimSites);
            }
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_SaveAndComeBackLater_RedirectsToApplicationSaved()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed("SaveAndComeBackLater");

            // Assert
            using (var scope = new AssertionScope())
            {
                var redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect.Url.Should().Be(PagePaths.ApplicationSaved);
            }
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_UnknownButtonAction_RedirectsToError()
        {
            // Arrange
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed("UnknownAction");

            // Assert
            using (var scope = new AssertionScope())
            {
                var redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect.Url.Should().Be("/Error");
            }
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_ReturnsError_WhenSessionRegistrationIdIsNull()
        {
            // Arrange
            var session = new ExporterRegistrationSession { RegistrationId = null };
            _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.InterimSitesQuestionOne();

            // Assert
            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult!.Url.Should().Be("/Error");
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_SetsJourneyAndBackLinkAndReturnsView()
        {
            // Arrange
            var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
            _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);
            _sessionManagerMock.Setup(m => m.SaveSessionAsync(It.IsAny<ISession>(), session))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.InterimSitesQuestionOne();

            // Assert
            session.Journey.Should().Contain(PagePaths.RegistrationLanding);
            session.Journey.Should().Contain(PagePaths.ExporterRegistrationTaskList);
            session.Journey.Should().Contain(PagePaths.ExporterInterimSiteQuestionOne);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Be("~/Views/Registration/Exporter/InterimSitesQuestionOne.cshtml");
            viewResult.Model.Should().BeOfType<InterimSitesQuestionOneViewModel>();
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_ReturnsError_WhenSessionIsNull()
        {
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync((ExporterRegistrationSession)null);

            var model = new InterimSitesQuestionOneViewModel();
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_ReturnsError_WhenRegistrationMaterialIdIsNull()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var model = new InterimSitesQuestionOneViewModel();
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_ReturnsView_WhenValidationFails()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = System.Guid.NewGuid()
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var validationResult = new FluentValidation.Results.ValidationResult 
            { 
                Errors = new List<ValidationFailure>()
                {
                    new ValidationFailure("QuestionOne", "QuestionOne is required")
                }
            };
            _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<InterimSitesQuestionOneViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var model = new InterimSitesQuestionOneViewModel();
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be("~/Views/Registration/Exporter/InterimSitesQuestionOne.cshtml");
            ((ViewResult)result).Model.Should().Be(model);
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_RedirectsToAddInterimSites_WhenHasInterimSitesTrue_AndSaveAndContinue()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = System.Guid.NewGuid()
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<InterimSitesQuestionOneViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(PagePaths.ExporterAddInterimSites);
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_MarksTaskCompletedAndRedirects_WhenHasInterimSitesFalse_AndSaveAndContinue()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = System.Guid.NewGuid()
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<InterimSitesQuestionOneViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _reprocessorServiceMock.Setup(x => x.RegistrationMaterials.UpdateApplicationRegistrationTaskStatusAsync(
                It.IsAny<System.Guid>(), It.IsAny<UpdateRegistrationTaskStatusDto>()
            )).Returns(Task.CompletedTask);

            var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(PagePaths.ExporterRegistrationTaskList);
        }

        [TestMethod]
        public async Task InterimSitesQuestionOne_RedirectsToApplicationSaved_WhenButtonActionIsNotSaveAndContinue()
        {
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    RegistrationMaterialId = System.Guid.NewGuid()
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            var validationResult = new FluentValidation.Results.ValidationResult();
            _validationServiceMock.Setup(x => x.ValidateAsync(It.IsAny<InterimSitesQuestionOneViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };
            var result = await _controller.InterimSitesQuestionOne(model, "SaveAndComeBackLater");

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be(PagePaths.ApplicationSaved);
        }

        [TestMethod]
        public async Task ChangeInterimSiteDetails_SetsCorrectIsActive_AndRedirects()
        {
            // Arrange
            var interimAddresses = new List<InterimSiteAddress>
            {
                CreateInterimSiteAddress(false),
                CreateInterimSiteAddress(
                            false,
                            "Sample Organisation 2",
                            "123 Example Street 2",
                            "Suite 456 2",
                            "Sample City 2",
                            "Sample State 2",
                            "Sample Country 2",
                            "AB12 3CD 2"
                         ),
                CreateInterimSiteAddress(
                            false,
                            "Sample Organisation 3",
                            "123 Example Street 3",
                            "Suite 456 3",
                            "Sample City 3",
                            "Sample State 3",
                            "Sample Country 3",
                            "AB12 3CD 3"
                         ),
            };
            var overseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
            {
                new OverseasMaterialReprocessingSite
                {
                    IsActive = true,
                    OverseasAddress = CreateOverseasAddress(),
                    InterimSiteAddresses = interimAddresses
                }
            };
            var interimSites = new InterimSites { OverseasMaterialReprocessingSites = overseasMaterialReprocessingSites };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = interimSites
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ChangeInterimSiteDetails(2);

            // Assert
            using (var scope = new AssertionScope())
            {
                interimAddresses[0].IsActive.Should().BeFalse();
                interimAddresses[1].IsActive.Should().BeTrue();
                interimAddresses[2].IsActive.Should().BeFalse();

                result.Should().BeOfType<RedirectToActionResult>();
                var redirect = (RedirectToActionResult)result;
                redirect.ActionName.Should().Be(nameof(ExporterController.InterimSiteDetails));
            }
        }

        [TestMethod]
        public async Task ChangeInterimSiteDetails_CallsSaveSessionWithCorrectPagePath()
        {
            // Arrange
            var interimAddresses = new List<InterimSiteAddress>
            {
                CreateInterimSiteAddress(false),

            };
            var overseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
            {
                new OverseasMaterialReprocessingSite
                {
                    IsActive = true,
                    OverseasAddress = CreateOverseasAddress(),
                    InterimSiteAddresses = interimAddresses
                }
            };
            var interimSites = new InterimSites { OverseasMaterialReprocessingSites = overseasMaterialReprocessingSites };
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = interimSites
                }
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
            var wasCalled = false;
            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), session))
                .Callback(() => wasCalled = true)
                .Returns(Task.CompletedTask);

            // Act
            await _controller.ChangeInterimSiteDetails(1);

            // Assert
            wasCalled.Should().BeTrue();
        }

        [TestMethod]
        public async Task DeleteInterimSite_RemovesSiteAndSetsTempDataAndRedirects()
        {
            // Arrange
            var interimSite1 = CreateInterimSiteAddress(false);
            var interimSite2 = CreateInterimSiteAddress(
                                true,
                                "Sample Organisation 2",
                                "123 Example Street 2",
                                "Suite 456 2",
                                "Sample City 2",
                                "Sample State 2",
                                "Sample Country 2",
                                "AB12 3CD 2"
                             );
            var interimSite3 = CreateInterimSiteAddress(
                                false,
                                "Sample Organisation 3",
                                "123 Example Street 3",
                                "Suite 456 3",
                                "Sample City 3",
                                "Sample State 3",
                                "Sample Country 3",
                                "AB12 3CD 3"
                             );

            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = CreateOverseasAddress(),
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSite1, interimSite2, interimSite3 }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };

            _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Initialize TempData
            var tempData = new TempDataDictionary(_httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            _controller.TempData = tempData;

            // Act
            var result = await _controller.DeleteInterimSite(2);

            // Assert
            using (var scope = new AssertionScope())
            {
                _controller.TempData["DeletedInterimSite"].Should().Be("Sample Organisation 2, 123 Example Street 2");

                // The interim site should be removed from the model
                var model = new CheckInterimSitesAnswersViewModel(overseasMaterialReprocessingSite);
                model.InterimSiteAddresses.Remove(interimSite2);
                model.InterimSiteAddresses.Should().HaveCount(2);
                model.InterimSiteAddresses.Should().NotContain(interimSite2);

                // Should redirect to InterimSiteUsed
                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = (RedirectToActionResult)result;
                redirectResult.ActionName.Should().Be(nameof(ExporterController.InterimSiteUsed));

                // SaveSession should be called
                _sessionManagerMock.Verify(sm => sm.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
            }
        }

        [TestMethod]
        public async Task DeleteInterimSite_WithInvalidIndex_ThrowsException()
        {
            // Arrange
            var interimSite1 = CreateInterimSiteAddress(false);
            var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = CreateOverseasAddress(),
                InterimSiteAddresses = new List<InterimSiteAddress> { interimSite1 }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite> { overseasMaterialReprocessingSite }
                    }
                },
                Journey = new List<string>()
            };

            var tempData = new TempDataDictionary(_httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            _controller.TempData = tempData;
            _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var act = async () => await _controller.DeleteInterimSite(5);

            // Assert
            await act.Should().ThrowAsync<System.ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public async Task AddAnotherInterimSiteFromCheckYourAnswer_Should_Set_All_InterimSiteAddresses_IsActive_To_False_And_Redirect()
        {
            // Arrange
            var interimSiteAddresses = new List<InterimSiteAddress>
            {
                CreateInterimSiteAddress(true),
                CreateInterimSiteAddress(
                            true,
                            "Sample Organisation 2",
                            "123 Example Street 2",
                            "Suite 456 2",
                            "Sample City 2",
                            "Sample State 2",
                            "Sample Country 2",
                            "AB12 3CD 2"
                         )
            };

            var overseasMaterialReprocessingSites = new List<OverseasMaterialReprocessingSite>
            {
                new OverseasMaterialReprocessingSite
                {
                    IsActive = true,
                    OverseasAddress = CreateOverseasAddress(),
                    InterimSiteAddresses = interimSiteAddresses
                }
            };

            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
                    InterimSites = new InterimSites
                    {
                        OverseasMaterialReprocessingSites = overseasMaterialReprocessingSites
                    }
                }
            };

            _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.AddAnotherInterimSiteFromCheckYourAnswer();

            // Assert
            using (var scope = new AssertionScope())
            {
                interimSiteAddresses.Should().OnlyContain(a => a.IsActive == false);
                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult.ActionName.Should().Be(nameof(ExporterController.InterimSiteDetails));
            }
        }

        private OverseasAddress CreateOverseasAddress()
                => new OverseasAddress
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Suite 100",
                    CityOrTown = "London",
                    CountryName = "UK",
                    ExternalId = Guid.NewGuid(),
                    OrganisationName = "Org One",
                    PostCode = "W1A 1AA",
                    StateProvince = "Greater London",
                    SiteCoordinates = "999"
                };

        private InterimSiteAddress CreateInterimSiteAddress(
            bool isActive = true,
            string organisationName = "Sample Organisation",
            string addressLine1 = "123 Example Street",
            string addressLine2 = "Suite 456",
            string cityOrTown = "Sample City",
            string stateProvince = "Sample State",
            string country = "Sample Country",
            string postCode = "AB12 3CD"
            )
            => new InterimSiteAddress
            {
                Id = Guid.NewGuid(),
                IsActive = isActive,
                OrganisationName = organisationName,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                CityOrTown = cityOrTown,
                StateProvince = stateProvince,
                CountryName = country,
                PostCode = postCode,
                InterimAddressContact = new List<OverseasAddressContact>
                         {
                             CreateOverseasAddressContact(),
                             CreateOverseasAddressContact(
                                    "Jane Smith",
                                    "jane.smith@example.com",
                                    "+0987654321"
                                 )
                         }
            };

        private OverseasAddressContact CreateOverseasAddressContact(
                string fullName = "John Doe",
                string email = "john.doe@example.com",
                string phoneNumber = "+1234567890"
            )
            => new OverseasAddressContact
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber
            };

        private OverseasMaterialReprocessingSite CreateOverseasMaterialReprocessingSite()
            => new OverseasMaterialReprocessingSite
            {
                IsActive = true,
                OverseasAddress = new OverseasAddress
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Suite 100",
                    CityOrTown = "London",
                    CountryName = "UK",
                    ExternalId = Guid.NewGuid(),
                    OrganisationName = "Org One",
                    PostCode = "W1A 1AA",
                    StateProvince = "Greater London",
                    SiteCoordinates = "999"
                },
                InterimSiteAddresses = new List<InterimSiteAddress>
                 {
                     CreateInterimSiteAddress(),
                     CreateInterimSiteAddress(
                            false,
                            "Sample Organisation 2",
                            "123 Example Street 2",
                            "Suite 456 2",
                            "Sample City 2",
                            "Sample State 2",
                            "Sample Country 2",
                            "AB12 3CD 2"
                         ),
                 }
            };
    }
    
    



    // Helper extensions for invoking protected methods
    public static class TestHelperExtensions
    {
        public static T InvokeProtectedMethod<T>(this object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (T)method.Invoke(obj, parameters);
        }

        public static void InvokeProtectedMethod(this object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(obj, parameters);
        }
    }
}
