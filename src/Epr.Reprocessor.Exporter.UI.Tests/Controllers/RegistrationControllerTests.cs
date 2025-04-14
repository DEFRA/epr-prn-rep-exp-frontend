using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using EPR.Common.Authorization.Models;
using EPR.Common.Authorization.Sessions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{
    [TestClass]
    public class RegistrationControllerTests
    {
        private RegistrationController _controller;
        private Mock<ILogger<RegistrationController>> _logger;
        private Mock<ISaveAndContinueService> _userJourneySaveAndContinueService;

        private ReprocessorExporterRegistrationSession _session;
        private Mock<ISessionManager<ReprocessorExporterRegistrationSession>> _sessionManagerMock;
        private readonly Mock<HttpContext> _httpContextMock = new();
        private readonly Mock<ClaimsPrincipal> _userMock = new();
        protected ITempDataDictionary TempDataDictionary;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<RegistrationController>>();
            _userJourneySaveAndContinueService = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ReprocessorExporterRegistrationSession>>();

            _controller = new RegistrationController(_logger.Object, _userJourneySaveAndContinueService.Object, _sessionManagerMock.Object);

            SetUpUserAndSessions();

            TempDataDictionary = new TempDataDictionary(this._httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            _controller.TempData = TempDataDictionary;
        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldReturnView()
        {
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x =>x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            // Act
            var result = await _controller.UKSiteLocation();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldSetBackLink()
        {
            // Act
            var result = await _controller.UKSiteLocation() as ViewResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<ViewResult>();
            backlink.Should().Be(PagePaths.AddressForLegalDocuments);
        }

        [TestMethod]
        public async Task UKSiteLocation_Get_ReturnsViewWithModel()
        {
            // Arrange
            var session = new ReprocessorExporterRegistrationSession { Journey = new List<string>() };
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.UKSiteLocation();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task UKSiteLocation_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new UKSiteLocationViewModel();
            _controller.ModelState.AddModelError("SiteLocationId", "Required");

            // Act
            var result = await _controller.UKSiteLocation(model, "SaveAndContinue");

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.Should().NotBeNull(); 
        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldSaveSession()
        {
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            // Act
            var result = await _controller.UKSiteLocation() as ViewResult;
            var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
            // Assert
            result.Should().BeOfType<ViewResult>();

            _sessionManagerMock.Verify(x=>x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

            _session.Journey.Count.Should().Be(2);
            _session.Journey[0].Should().Be(PagePaths.AddressForLegalDocuments);
            _session.Journey[1].Should().Be(PagePaths.CountryOfReprocessingSite);
        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldSetFromSaveAndContinue()
        {
            var expetcedModel = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new  SaveAndContinueResponseDto
            {
                Action = nameof(RegistrationController.UKSiteLocation),
                Controller = nameof(RegistrationController),
                Area = SaveAndContinueAreas.Registration,
                CreatedOn = DateTime.UtcNow,
                Id = 1,
                RegistrationId = 1,
                Parameters = JsonConvert.SerializeObject(expetcedModel)
            });

            // Act
            var result = await _controller.UKSiteLocation() as ViewResult;
            var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
            var model = result.Model as UKSiteLocationViewModel;

            // Assert
            result.Should().BeOfType<ViewResult>();
            _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

            model.Should().BeEquivalentTo(expetcedModel);
        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldSetStubTempDataSaveAndContinue()
        {
            var expetcedModel = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            _controller.TempData["SaveAndContinueUkSiteNationKey"] = JsonConvert.SerializeObject(expetcedModel);

            _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((SaveAndContinueResponseDto)null);

            // Act
            var result = await _controller.UKSiteLocation() as ViewResult;
            var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
            var model = result.Model as UKSiteLocationViewModel;

            // Assert
            result.Should().BeOfType<ViewResult>();
            _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

            model.Should().BeEquivalentTo(expetcedModel);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_ShouldValidateModel()
        {
            var saveAndContinue = "SaveAndContinue";
            var model = new UKSiteLocationViewModel() { SiteLocationId = null };
            var expectedErrorMessage = "Select the country the reprocessing site is located in.";
            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model, saveAndContinue);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<ViewResult>();

            Assert.IsTrue(modelState["SiteLocationId"].Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, modelState["SiteLocationId"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldRedirectNextPage()
        {
            var saveAndContinue = "SaveAndContinue";
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model, saveAndContinue) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();

            result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldSetBackLink()
        {
            var saveAndContinue = "SaveAndContinue";
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite} };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model, saveAndContinue) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();

            backlink.Should().Be(PagePaths.AddressForLegalDocuments);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldRedirectNextPage()
        {
            var saveAndComeBackLater = "SaveAndComeBackLater";
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.UKSiteLocation(model, saveAndComeBackLater) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();
            result.Url.Should().Be(PagePaths.ApplicationSaved);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldSetBackLink()
        {
            var saveAndComeBackLater = "SaveAndComeBackLater";
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.UKSiteLocation(model, saveAndComeBackLater) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();
            backlink.Should().Be(PagePaths.AddressForLegalDocuments);
        }

        [TestMethod]
        public void NoAddressFound_ShouldReturnViewWithModel()
        {
            var result = _controller.NoAddressFound() as ViewResult;
            var model = result.Model as NoAddressFoundViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
            model.Postcode.Should().Be("[TEST POSTCODE REPLACE WITH SESSION]");
        }

        [TestMethod]
        public void PostcodeOfReprocessingSite_Get_ShouldReturnViewWithModel()
        {
            var result = _controller.PostcodeOfReprocessingSite() as ViewResult;
            var model = result.Model as PostcodeOfReprocessingSiteViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
        }

        [TestMethod]
        public void PostcodeOfReprocessingSite_Post_ShouldReturnViewWithModel()
        {
            var model = new PostcodeOfReprocessingSiteViewModel();
            var result = _controller.PostcodeOfReprocessingSite(model) as ViewResult;

            result.Should().BeOfType<ViewResult>();
            result.Model.Should().Be(model);
        }

        [TestMethod]
        public void AddressOfReprocessingSite_Get_ShouldReturnViewWithModel()
        {
            // Act
            var result = _controller.AddressOfReprocessingSite() as ViewResult;
            var model = result.Model as AddressOfReprocessingSiteViewModel;

            // Assert
            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
            model.AddressLine1.Should().Be("Test Data House");
            model.AddressLine2.Should().Be("123 Test Data Lane");
            model.TownCity.Should().Be("Test Data City");
            model.County.Should().Be("Test County");
            model.Postcode.Should().Be("TST 123");
        }

        [TestMethod]
        public void AddressOfReprocessingSite_Post_ValidModel_ShouldThrowNotImplementedException()
        {
            // Arrange
            var model = new AddressOfReprocessingSitePostModel
            {
                IsSameAddress = true
            };

            ValidateViewModel(model);

            // Act & Assert
            Assert.ThrowsException<NotImplementedException>(() => _controller.AddressOfReprocessingSite(model));
        }

        [TestMethod]
        public void AddressOfReprocessingSite_Post_InvalidModel_ShouldReturnViewWithDefaultModel()
        {
            // Arrange
            var model = new AddressOfReprocessingSitePostModel
            {
                IsSameAddress = null
            };

            // Act
            var result = _controller.AddressOfReprocessingSite(model) as ViewResult;
            var returnedModel = result.Model as AddressOfReprocessingSiteViewModel;

            // Assert
            result.Should().BeOfType<ViewResult>();
            returnedModel.Should().NotBeNull();
        }

        [TestMethod]
        public void AddressOfReprocessingSite_Post_InvalidModel_ShouldPreserveModelStateErrors()
        {
            // Arrange
            var model = new AddressOfReprocessingSitePostModel
            {
                IsSameAddress = null
            };
            // Act
            ValidateViewModel(model);

            var result = _controller.AddressOfReprocessingSite(model) as ViewResult;

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.ErrorCount.Should().Be(1);
            _controller.ModelState["IsSameAddress"].Errors[0].ErrorMessage.Should().Be("Select an address for the reprocessing site");            
        }


        private void ValidateViewModel(object Model)
        {
            ValidationContext validationContext = new ValidationContext(Model, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(Model, validationContext, validationResults, true);
            foreach (ValidationResult validationResult in validationResults)
            {
                _controller.ControllerContext.ModelState.AddModelError(String.Join(", ", validationResult.MemberNames), validationResult.ErrorMessage);
            }
        }

        private void SetUpUserAndSessions()
        {
            _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).Returns(Task.FromResult(new ReprocessorExporterRegistrationSession()));

            var claims = new List<Claim>();
            var userData = GetUserData();
            if (userData != null)
            {
                claims.Add(new(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)));
            }

            _userMock.Setup(x => x.Claims).Returns(claims);
            _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;
        }

        private static UserData GetUserData()
        {
            return new UserData
            {
                Id = Guid.NewGuid(),
                Organisations = new()
            {
                new()
                {
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            }
            };
        }
    }
}
