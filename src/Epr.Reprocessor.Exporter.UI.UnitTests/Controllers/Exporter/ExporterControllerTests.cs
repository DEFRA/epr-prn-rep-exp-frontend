using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Controllers;
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
            _controller = new ExporterController(
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _registrationServiceMock.Object,
                _validationServiceMock.Object

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
            var result = await _controller.Index();

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
            var result = await _controller.Index();

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
            var result = await _controller.Index();

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
            _controller.ModelState.AddModelError("Country", "Required");
            var model = new OverseasReprocessorSiteViewModel();
            var countries = new List<string> { "UK" };
            _registrationServiceMock.Setup(x => x.GetCountries())
                .ReturnsAsync(countries);

            // Act
            var result = await _controller.Index(model, "SaveAndContinue");

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
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);

            // Act
            var result = await _controller.Index(model, "SaveAndContinue");

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
            var result = await _controller.Index(model, "SaveAndContinue");

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
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(session);
            _mapperMock.Setup(x => x.Map<OverseasAddress>(model))
                .Returns(mappedAddress);

            // Act
            var result = await _controller.Index(model, "SaveAndContinue");

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
            var result = await _controller.Index(model, "SaveAndContinue");

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
            var result = await _controller.Index(model, "SaveAndContinue");

            // Assert
            using (var scope = new AssertionScope())
            {
                overseasSites.OverseasAddresses.Should().NotBeNull();
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
            var result = await _controller.Index();

            // Assert
            using (var scope = new AssertionScope())
            {
                session.Journey.Should().Contain("test-setup-session");
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
            var result = await _controller.Index();

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

            // Act
            var result = await _controller.Index(model, "SaveAndContinue");

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
