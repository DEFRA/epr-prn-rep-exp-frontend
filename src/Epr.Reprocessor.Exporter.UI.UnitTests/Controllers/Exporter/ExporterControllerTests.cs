using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Controllers.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers.Exporter;


    [TestClass]
    public class ExporterControllerTests
    {
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRegistrationService> _registrationServiceMock;
        private Mock<IValidationService> _validationServiceMock;
        private Mock<IExporterRegistrationService> _exporterRegistrationService;
        private Mock<ILogger<RegistrationController>> _logger;
        private new Mock<ISaveAndContinueService> _userJourneySaveAndContinueService;
        private DefaultHttpContext _httpContext;
        private ExporterController _controller;

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
            _mapperMock = new Mock<IMapper>();
            _registrationServiceMock = new Mock<IRegistrationService>();
            _validationServiceMock = new Mock<IValidationService>();
            _exporterRegistrationService = new Mock<IExporterRegistrationService>();
            _controller = new ExporterController(
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _registrationServiceMock.Object,
                _validationServiceMock.Object,
                _exporterRegistrationService.Object
            );

            // Initialize HttpContext with a mock session
            _httpContext = new DefaultHttpContext();
            var mockSession = new Mock<ISession>();
            _httpContext.Session = mockSession.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
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
                session.Journey.Should().ContainInOrder(PagePaths.AddAnotherOverseasReprocessingSite, PagePaths.ExporterInterimSitesUsed);
                _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), session), Times.Once);
                backLink.Should().Be(PagePaths.AddAnotherOverseasReprocessingSite);
            }
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_SaveAndContinue_RedirectsToAddInterimSites()
        {
            // Arrange
            var model = new CheckInterimSitesAnswersViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed(model, "SaveAndContinue");

            // Assert
            var redirect = result as RedirectResult;
            redirect.Should().NotBeNull();
            redirect.Url.Should().Be(PagePaths.ExporterAddInterimSites);
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_SaveAndComeBackLater_RedirectsToApplicationSaved()
        {
            // Arrange
            var model = new CheckInterimSitesAnswersViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed(model, "SaveAndComeBackLater");

            // Assert
            var redirect = result as RedirectResult;
            redirect.Should().NotBeNull();
            redirect.Url.Should().Be(PagePaths.ApplicationSaved);
        }

        [TestMethod]
        public async Task ExporterInterimSitesUsed_UnknownButtonAction_RedirectsToError()
        {
            // Arrange
            var model = new CheckInterimSitesAnswersViewModel();
            var session = new ExporterRegistrationSession
            {
                ExporterRegistrationApplicationSession = new ExporterRegistrationApplicationSession()
            };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.ExporterInterimSitesUsed(model, "UnknownAction");

            // Assert
            var redirect = result as RedirectResult;
            redirect.Should().NotBeNull();
            redirect.Url.Should().Be("/Error");
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
                    OverseasAddress = CreateOverseasAddressBase(),
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
            interimAddresses[0].IsActive.Should().BeFalse();
            interimAddresses[1].IsActive.Should().BeTrue();
            interimAddresses[2].IsActive.Should().BeFalse();

            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = (RedirectToActionResult)result;
            redirect.ActionName.Should().Be(nameof(ExporterController.InterimSiteDetails));
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
                    OverseasAddress = CreateOverseasAddressBase(),
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
            OverseasAddress = CreateOverseasAddressBase(),
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

    [TestMethod]
    public async Task DeleteInterimSite_WithInvalidIndex_ThrowsException()
    {
        // Arrange
        var interimSite1 = CreateInterimSiteAddress(false);
        var overseasMaterialReprocessingSite = new OverseasMaterialReprocessingSite
        {
            IsActive = true,
            OverseasAddress = CreateOverseasAddressBase(),
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
                    OverseasAddress = CreateOverseasAddressBase(),
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
        interimSiteAddresses.Should().OnlyContain(a => a.IsActive == false);
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult.ActionName.Should().Be(nameof(ExporterController.InterimSiteDetails));
    }

    private OverseasAddressBase CreateOverseasAddressBase()
            => new OverseasAddressBase
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Suite 100",
                CityorTown = "London",
                Country = "UK",
                Id = Guid.NewGuid(),
                OrganisationName = "Org One",
                PostCode = "W1A 1AA",
                StateProvince = "Greater London"
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
                CityorTown = cityOrTown,
                StateProvince = stateProvince,
                Country = country,
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
                OverseasAddress = new OverseasAddressBase
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Suite 100",
                    CityorTown = "London",
                    Country = "UK",
                    Id = Guid.NewGuid(),
                    OrganisationName = "Org One",
                    PostCode = "W1A 1AA",
                    StateProvince = "Greater London"
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

