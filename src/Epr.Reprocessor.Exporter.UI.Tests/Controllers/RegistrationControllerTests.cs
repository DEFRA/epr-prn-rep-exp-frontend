using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using EPR.Common.Authorization.Models;
using EPR.Common.Authorization.Sessions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<RegistrationController>>();
            _userJourneySaveAndContinueService = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ReprocessorExporterRegistrationSession>>();

            _controller = new RegistrationController(_logger.Object, _userJourneySaveAndContinueService.Object, _sessionManagerMock.Object);

            SetUpUserAndSessions();
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
        public async Task UkSiteLocation_OnSubmit_ShouldValidateModel()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = null };
            var expectedErrorMessage = "Select the country the reprocessing site is located in.";
            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<ViewResult>();

            Assert.IsTrue(modelState["SiteLocationId"].Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, modelState["SiteLocationId"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_ShouldRedirectNextPage()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();

            result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_ShouldSetBackLink()
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite} };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();

            backlink.Should().Be(PagePaths.AddressForLegalDocuments);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmitSaveAndContinue_ShouldRedirectNextPage()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.UKSiteLocationSaveAndContinue(model) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();
            result.Url.Should().Be(PagePaths.ApplicationSaved);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmitSaveAndContinue_ShouldSetBackLink()
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.UKSiteLocationSaveAndContinue(model) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();
            backlink.Should().Be(PagePaths.AddressForLegalDocuments);
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
