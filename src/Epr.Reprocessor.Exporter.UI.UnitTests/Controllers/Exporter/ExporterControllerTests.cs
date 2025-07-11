using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Controllers.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation.Results;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Exporter;

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
            CityorTown = "Default City",
            Country = "Default Country",
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
            CityorTown = "Default City",
            Country = "Default Country",
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
            CityorTown = "Default City",
            Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
            CityorTown = "City",
            Country = "Country",
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
            CityorTown = "",
            Country = "",
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
                                CityorTown = "",
                                Country = "",
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
                                CityorTown = "",
                                Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
        var result = _controller.AddAnotherOverseasReprocessingSite(model, SaveAndContinueActionKey);
        var view = await result as RedirectResult;

        // Assert
        view.Url.Should().BeEquivalentTo(redirectToUrl);
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
            CityorTown = "",
            Country = "",
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
    public async Task CheckOverseasReprocessingSitesAnswers_SaveAndContinueWithAddresses_SavesAndRedirectsToLanding()
    {
        // Arrange
        var session = CreateSessionWithAddresses(2);
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        var model = new CheckOverseasReprocessingSitesAnswersViewModel();
        var buttonAction = "SaveAndContinue";
        var dto = new OverseasAddressRequestDto();
        _mapperMock.Setup(m => m.Map<OverseasAddressRequestDto>(It.IsAny<ExporterRegistrationApplicationSession>())).Returns(dto);
        _exporterRegistrationService.Setup(e => e.SaveOverseasReprocessorAsync(dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CheckOverseasReprocessingSitesAnswers(model, buttonAction);

            // Assert
            using (var scope = new AssertionScope())
            {
                var redirect = result as RedirectResult;
                redirect.Should().NotBeNull();
                redirect.Url.Should().Be(PagePaths.ExporterTaskList);
                _exporterRegistrationService.Verify(e => e.SaveOverseasReprocessorAsync(dto), Times.Once);
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
                    OverseasAddress = new OverseasAddressBase
                    {
                        Id = dto.OverseasAddress.Id,
                        OrganisationName = dto.OverseasAddress.OrganisationName,
                        AddressLine1 = dto.OverseasAddress.AddressLine1,
                        AddressLine2 = dto.OverseasAddress.AddressLine2,
                        CityorTown = dto.OverseasAddress.CityorTown,
                        Country = dto.OverseasAddress.Country,
                        PostCode = dto.OverseasAddress.PostCode,
                        StateProvince = dto.OverseasAddress.StateProvince
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
                           OverseasAddress = new OverseasAddressBase
                           {
                               Id = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                               OrganisationName = "Org 1",
                               AddressLine1 = "Address 1",
                               AddressLine2 = "Address line 2",
                               CityorTown = "City 1",
                               Country = "Portugal",
                               PostCode = "POSTCODE001",
                               StateProvince = "Lisbon"
                           },
                           OverseasAddressId = Guid.NewGuid()
                       });

            var result = await _controller.AddInterimSites("SaveAndContinue");

            // Note: method ends in a Redirect that is not returned (logical bug in source method)
            result.Should().BeOfType<RedirectToActionResult>()
                  .Which.ActionName.Should().Be(nameof(_controller.AddInterimSites));
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
                                OverseasAddress = new OverseasAddressBase
                                {
                                    Id = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                                    OrganisationName = "Org 1",
                                    AddressLine1 = "Address 1",
                                    AddressLine2 = "Address line 2",
                                    CityorTown = "City 1",
                                    Country = "Portugal",
                                    PostCode = "POSTCODE001",
                                    StateProvince = "Lisbon"
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
                                        CityorTown = "Paris",
                                        Country = "France",
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
                                        CityorTown = "Warshaw",
                                        Country = "Poland",
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
                                OverseasAddress = new OverseasAddressBase
                                {
                                    Id = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                                    OrganisationName = "Org 1",
                                    AddressLine1 = "Address 1",
                                    AddressLine2 = "Address 2",
                                    CityorTown = "Munich",
                                    Country = "Germany",
                                    PostCode = "0932131",
                                    StateProvince = "some state"
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
                                        CityorTown = "Paris",
                                        Country = "France",
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
                    OverseasAddress = new OverseasAddressBaseDto()
                    {
                        Id = new Guid("C6E1E794-13A1-4114-9019-B1A1055ED907"),
                        OrganisationName = "Org 1",
                        AddressLine1 = "Address 1",
                        AddressLine2 = "Address line 2",
                        CityorTown = "City 1",
                        Country = "Portugal",
                        PostCode = "POSTCODE001",
                        StateProvince = "Lisbon"
                    },
                    InterimSiteAddresses = null

                },
                new OverseasMaterialReprocessingSiteDto
                {
                    Id = new Guid("A1B2C3D4-E5F6-7890-1234-56789ABCDEF0"),
                    OverseasAddressId = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                    OverseasAddress = new OverseasAddressBaseDto()
                    {
                        Id = new Guid("399AE234-2227-4C11-BBC1-3F7F2DF69936"),
                        OrganisationName = "Org 1",
                        AddressLine1 = "Address 1",
                        AddressLine2 = "Address 2",
                        CityorTown = "Munich",
                        Country = "Germany",
                        PostCode = "0932131",
                        StateProvince = "some state"
                    },
                    InterimSiteAddresses = new List<InterimSiteAddressDto>
                    {
                        new InterimSiteAddressDto //existing record will be updated via session
                        {
                            Id = new Guid("38296DC3-B9BC-41D8-B886-EDA6450F35A3"),
                            OrganisationName = "Interim Org 1",
                            AddressLine1 = "Interim Address 1",
                            AddressLine2 = "Interim Address 2",
                            CityorTown = "Paris",
                            Country = "France",
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
                        OverseasAddress = new OverseasAddressBaseDto
                        {
                            Id = new Guid("6033F5CD-E2C0-4AB5-BFAF-C5F517E93EAE"),
                            OrganisationName = "New Org from DB",
                            AddressLine1 = "Address 1",
                            AddressLine2 = "Address 2",
                            CityorTown = "Oslo",
                            Country = "Norway",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "",
                Country = "",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                CityorTown = "Test City",
                Country = "Test Country",
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
                    CityorTown = $"City_{i}",
                    Country = $"Country_{i}",
                    PostCode = $"PostCode_{i}",
                    SiteCoordinates = $"Coordinates_{i}",
                    StateProvince = $"State_{i}"
                });
            }
            return new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession
                {
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",            
                    Country = "Default Country",            
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",
                    Country = "Default Country",
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
                new() { PropertyName = "Country", ErrorMessage = "Country Missing" }
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
                CityorTown = "Default City",
                Country = "Default Country",
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
                    CityorTown = "Default City",            
                    Country = "Default Country",            
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
                CityorTown = "Default City",
                Country = "Default Country",
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
            CityorTown = "Default City",
            Country = "Default Country",
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
    

    [Ignore("This test will fail until the page that is redirected to is developed")]
    [TestMethod]
    public async Task Post_InterimSitesQuestionOne_SaveAndContinue_With_HasInterimSites_False_Redirects()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };
        var session = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        // Assert
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(redirectResult.Url.Contains("tasklist7", StringComparison.OrdinalIgnoreCase));//needs updating once page exists
    }

    [TestMethod]
    public async Task Post_InterimSitesQuestionOne_SaveAndComeBackLater_Redirects_To_ApplicationSaved()
    {
        // Arrange
        var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };

        var validationResult = new FluentValidation.Results.ValidationResult();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndComeBackLater");

        // Assert
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(redirectResult.Url.Contains(PagePaths.ApplicationSaved, StringComparison.OrdinalIgnoreCase));//needs updating once page exists
    }

    [TestMethod]
    public async Task Get_InterimSitesQuestionOne_Get_ReturnsError_WhenSessionIsNull()
    {
        // Arrange
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ExporterRegistrationSession)null);

        // Act
        var result = await _controller.InterimSitesQuestionOne();

        // Assert
        using (var scope = new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/Error");
        }
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_InvalidModel_ReturnsViewWithModel()
    {
        var model = new InterimSitesQuestionOneViewModel();

        var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new() { PropertyName = "HasInterimStes", ErrorMessage = "Please select an option" }
            });

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        var view = result as ViewResult;
        view.Should().NotBeNull();
        view!.Model.Should().Be(model);
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_NullSession_RedirectsToError()
    {
        _sessionManagerMock
            .Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ExporterRegistrationSession?)null);

        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };

        var validationResult = new FluentValidation.Results.ValidationResult();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        var redirect = result as RedirectResult;
        redirect.Should().NotBeNull();
        redirect!.Url.Should().Be("/Error");
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_WithInterimSites_ContinuesToConfirmSite()
    {
        var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };

        var validationResult = new FluentValidation.Results.ValidationResult();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        var redirect = result as RedirectResult;
        redirect.Should().NotBeNull();
        redirect!.Url.Should().Be(PagePaths.ExporterAddInterimSites);
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_NoInterimSites_MarksTaskCompleteAndReturnsTaskList()
    {
        var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        _registrationMaterialServiceMock
            .Setup(r => r.UpdateApplicationRegistrationTaskStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateRegistrationTaskStatusDto>()
            ))
            .Returns(Task.CompletedTask);

        _reprocessorServiceMock
            .Setup(r => r.RegistrationMaterials)
            .Returns(_registrationMaterialServiceMock.Object);

        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };

        var validationResult = new FluentValidation.Results.ValidationResult();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        var redirect = result as RedirectResult;
        redirect.Should().NotBeNull();
        redirect!.Url.Should().Be(PagePaths.ExporterRegistrationTaskList);
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_SaveAndComeBackLater_RedirectsToSaved()
    {
        var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };

        var validationResult = new FluentValidation.Results.ValidationResult();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndComeBackLater");

        var redirect = result as RedirectResult;
        redirect.Should().NotBeNull();
        redirect!.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    [TestMethod]
    public async Task InterimSitesQuestionOne_ReturnsView_WhenSessionIsValid()
    {
        // Arrange
        var session = new ExporterRegistrationSession { RegistrationId = Guid.NewGuid() };
        session.ExporterRegistrationApplicationSession.RegistrationMaterialId = Guid.NewGuid();
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock
            .Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.InterimSitesQuestionOne();

        // Assert
        var view = result as ViewResult;
        Assert.IsNotNull(view);
        Assert.AreEqual("~/Views/Registration/Exporter/InterimSitesQuestionOne.cshtml", view.ViewName);
        Assert.IsInstanceOfType(view.Model, typeof(InterimSitesQuestionOneViewModel));
    }

    [TestMethod]
    public void HasInterimSites_Should_Set_And_Get_Value()
    {
        var interimSites = new InterimSites { HasInterimSites = true };
        Assert.IsTrue(interimSites.HasInterimSites.Value);
    }

    [TestMethod]
    public void OverseasMaterialReprocessingSites_Should_Initialize_As_Empty_List()
    {
        var interimSites = new InterimSites();
        Assert.IsNotNull(interimSites.OverseasMaterialReprocessingSites);
        Assert.AreEqual(0, interimSites.OverseasMaterialReprocessingSites.Count);
    }


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

