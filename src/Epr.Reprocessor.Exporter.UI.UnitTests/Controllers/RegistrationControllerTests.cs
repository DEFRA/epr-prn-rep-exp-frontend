using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using EPR.Common.Authorization.Models;
using EPR.Common.Authorization.Sessions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;


namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    [TestClass]
    public class RegistrationControllerTests
    {
        private RegistrationController _controller;
        private Mock<ILogger<RegistrationController>> _logger;
        private Mock<UI.App.Services.Interfaces.ISaveAndContinueService> _userJourneySaveAndContinueService;
        private Mock<IValidationService> _validationService;
        private ReprocessorExporterRegistrationSession _session;
        private Mock<ISessionManager<ReprocessorExporterRegistrationSession>> _sessionManagerMock;
        private readonly Mock<HttpContext> _httpContextMock = new();
        private readonly Mock<ClaimsPrincipal> _userMock = new();
        private  Mock<IStringLocalizer<RegistrationController>> _mockLocalizer = new();
        protected ITempDataDictionary TempDataDictionary;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<RegistrationController>>();
            _userJourneySaveAndContinueService = new Mock<UI.App.Services.Interfaces.ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ReprocessorExporterRegistrationSession>>();
            _validationService = new Mock<IValidationService>();

            _controller = new RegistrationController(_logger.Object, _userJourneySaveAndContinueService.Object, _sessionManagerMock.Object, _validationService.Object);

            SetUpUserAndSessions();

            TempDataDictionary = new TempDataDictionary(this._httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            _controller.TempData = TempDataDictionary;
        }


        [TestMethod]
        public async Task TaskList_ReturnsExpectedTaskListModel()
        {
            // Act
            var result = await _controller.TaskList() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            var model = result.Model as TaskListModel;
            Assert.IsNotNull(model, "Model should not be null");
            Assert.IsNotNull(model.TaskList, "TaskList should not be null");
            Assert.AreEqual(4, model.TaskList.Count, "TaskList should contain 4 items");

            // Verify each task item
            Assert.AreEqual("Site address and contact details", model.TaskList[0].TaskName);
            Assert.AreEqual("#", model.TaskList[0].TaskLink);
            Assert.AreEqual(TaskListStatus.NotStart, model.TaskList[0].status);

            Assert.AreEqual("Waste licenses, permits and exemptions", model.TaskList[1].TaskName);
            Assert.AreEqual("#", model.TaskList[1].TaskLink);
            Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[1].status);

            Assert.AreEqual("Reprocessing inputs and outputs", model.TaskList[2].TaskName);
            Assert.AreEqual("#", model.TaskList[2].TaskLink);
            Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[2].status);

            Assert.AreEqual("Sampling and inspection plan per material", model.TaskList[3].TaskName);
            Assert.AreEqual("#", model.TaskList[3].TaskLink);
            Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[3].status);
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
        public async Task NoAddressFound_ShouldReturnViewWithModel()
        {
            var result = await _controller.NoAddressFound() as ViewResult;
            var model = result.Model as NoAddressFoundViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
            model.Postcode.Should().Be("[TEST POSTCODE REPLACE WITH SESSION]");
        }

        [TestMethod]
        public async Task PostcodeOfReprocessingSite_Get_ShouldReturnViewWithModel()
        {
            var result = await _controller.PostcodeOfReprocessingSite() as ViewResult;
            var model = result.Model as PostcodeOfReprocessingSiteViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
        }

        [TestMethod]
        public async Task PostcodeOfReprocessingSite_Post_ShouldReturnViewWithModel()
        {
            var model = new PostcodeOfReprocessingSiteViewModel();
            var result = await _controller.PostcodeOfReprocessingSite(model) as ViewResult;

            result.Should().BeOfType<ViewResult>();
            result.Model.Should().Be(model);
        }

        [TestMethod]
        public async Task ProvideSiteGridReference_ShouldReturnView()
        {
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            // Act
            var result = await _controller.ProvideSiteGridReference();
            var model = (result as ViewResult).Model;

            // Assert
            result.Should().BeOfType<ViewResult>();
            model.Should().BeOfType<ProvideSiteGridReferenceViewModel>();
        }

        [TestMethod]
        public async Task ProvideSiteGridReference_ShouldSetBackLink()
        {
            // Act
            var result = await _controller.ProvideSiteGridReference() as ViewResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;

            // Assert
            result.Should().BeOfType<ViewResult>();

            backlink.Should().Be(PagePaths.AddressOfReprocessingSite);
        }

        [TestMethod]
        [DataRow(null, "Enter the site’s grid reference")]
        [DataRow("T%%", "Grid references must include numbers")]
        [DataRow("TF333", "Enter a grid reference with at least 4 numbers")]
        [DataRow("TF32141934322332", "Enter a grid reference with no more than 10 numbers")]
        public async Task ProvideSiteGridReference_OnSubmit_ValidateGridReference_ShouldValidateModel(string gridReference, string expectedErrorMessage)
        {
            var saveAndContinue = "SaveAndContinue";
            var model = new ProvideSiteGridReferenceViewModel() { GridReference = gridReference };
            ValidateViewModel(model);

            // Act
            var result = await _controller.ProvideSiteGridReference(model, saveAndContinue);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<ViewResult>();

            var modelStateErrorCount = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors.Count : modelState[""].Errors.Count;
            var modelStateErrorMessage = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors[0].ErrorMessage : modelState[""].Errors[0].ErrorMessage;

            Assert.AreEqual(1, modelStateErrorCount);
            Assert.AreEqual(expectedErrorMessage, modelStateErrorMessage);
        }

        [TestMethod]
        [DataRow("TF123434")]
        [DataRow("TF3333")]
        [DataRow("TF3214193478")]
        public async Task ProvideSiteGridReference_OnSubmit_ShouldBeSuccessful(string gridReference)
        {
            var saveAndContinue = "SaveAndContinue";
            var model = new ProvideSiteGridReferenceViewModel() { GridReference = gridReference };
            ValidateViewModel(model);

            // Act
            var result = await _controller.ProvideSiteGridReference(model, saveAndContinue);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<RedirectResult>();
        }

        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.AddressOfReprocessingSite)]
        [DataRow("SaveAndComeBackLater", PagePaths.AddressOfReprocessingSite)]
        public async Task ProvideSiteGridReference_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new ProvideSiteGridReferenceViewModel() { GridReference = "1245412545" };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            await _controller.ProvideSiteGridReference(model, actionButton);
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert

            backlink.Should().Be(backLinkUrl);
        }

        [TestMethod]
        [DataRow("SaveAndContinue", "/")]
        [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
        public async Task ProvideSiteGridReference_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { "/", PagePaths.GridReferenceForEnteredReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new ProvideSiteGridReferenceViewModel() { GridReference = "1245412545" };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.ProvideSiteGridReference(model, actionButton) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();
            result.Url.Should().Be(expectedReturnUrl);
        }

        [TestMethod]
        public async Task ManualAddressForServiceOfNotices_Get_ReturnsViewWithModel()
        {
            // Arrange
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForServiceOfNotices();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be("ManualAddressForServiceOfNotices");
                viewResult.Model.Should().BeOfType<ManualAddressForServiceOfNoticesViewModel>();
            }
        }


        [TestMethod]
        public async Task ManualAddressForServiceOfNotices_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ManualAddressForServiceOfNoticesViewModel();
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.Model.Should().Be(model);
            }
        }

        [TestMethod]
        public async Task ManualAddressForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
        {
            // Arrange
            var model = new ManualAddressForServiceOfNoticesViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.RegistrationLanding);
            }
        }

        [TestMethod]
        public async Task ManualAddressForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
        {
            // Arrange
            var model = new ManualAddressForServiceOfNoticesViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndComeBackLater");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
            }
        }

        [TestMethod]
        public async Task ProvideGridReferenceOfReprocessingSite_ShouldReturnView()
        {
            _session = new ReprocessorExporterRegistrationSession();
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            // Act
            var result = await _controller.ProvideGridReferenceOfReprocessingSite();
            var model = (result as ViewResult).Model;

            // Assert
            result.Should().BeOfType<ViewResult>();
            model.Should().BeOfType<ProvideGridReferenceOfReprocessingSiteViewModel>();
        }

        [TestMethod]
        public async Task ProvideGridReferenceOfReprocessingSite_ShouldSetBackLink()
        {
            // Act
            var result = await _controller.ProvideGridReferenceOfReprocessingSite() as ViewResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;

            // Assert
            result.Should().BeOfType<ViewResult>();

            backlink.Should().Be(PagePaths.CountryOfReprocessingSite);
        }

        [TestMethod]
        [DataRow(null, "Enter the site’s grid reference")]
        [DataRow("T$", "Grid references must include numbers")]
        [DataRow("T343", "Enter a grid reference with at least 4 numbers")]
        [DataRow("TF234323456782", "Enter a grid reference with no more than 10 numbers")]
        public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ValidateGridReference_ShouldValidateModel(string gridReference, string expectedErrorMessage)
        {
            var saveAndContinue = "SaveAndContinue";
            var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = gridReference };
            ValidateViewModel(model);

            // Act
            var result = await _controller.ProvideGridReferenceOfReprocessingSite(model, saveAndContinue);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<ViewResult>();

            var modelStateErrorCount = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors.Count : modelState[""].Errors.Count;
            var modelStateErrorMessage = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors[0].ErrorMessage : modelState[""].Errors[0].ErrorMessage;

            Assert.AreEqual(1, modelStateErrorCount);
            Assert.AreEqual(expectedErrorMessage, modelStateErrorMessage);
        }

        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.CountryOfReprocessingSite)]
        [DataRow("SaveAndComeBackLater", PagePaths.CountryOfReprocessingSite)]
        public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = "TS1245412545" };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton);
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert

            backlink.Should().Be(backLinkUrl);
        }

        [TestMethod]
        [DataRow("SaveAndContinue", "/")]
        [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
        public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
        {
            _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { "/", PagePaths.GridReferenceOfReprocessingSite } };
            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

            var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = "1245412545" };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton) as RedirectResult;
            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            // Assert
            result.Should().BeOfType<RedirectResult>();
            result.Url.Should().Be(expectedReturnUrl);
        }

        [TestMethod]
        public async Task SelectAddressForServiceOfNotices_Get_ReturnsViewWithModel()
        {
            // Arrange
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.SelectAddressForServiceOfNotices();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be("SelectAddressForServiceOfNotices");
                viewResult.Model.Should().BeOfType<SelectAddressForServiceOfNoticesViewModel>();
            }
        }

        [TestMethod]
        public async Task SelectedAddressForServiceOfNotices_Get_SaveAndContinue_RedirectsCorrectly()
        {
            // Arrange
            var selectedIndex = 0;
            var postcode = "G5 0US";

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.SelectedAddressForServiceOfNotices(postcode, selectedIndex);
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.RegistrationLanding);
            }
        }


        [TestMethod]
        public async Task ManualAddressForReprocessingSite_Get_ReturnsViewWithModel()
        {
            // Arrange
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForReprocessingSite();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be("ManualAddressForReprocessingSite");
                viewResult.Model.Should().BeOfType<ManualAddressForReprocessingSiteViewModel>();
            }
        }


        [TestMethod]
        public async Task ManualAddressForReprocessingSite_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ManualAddressForReprocessingSiteViewModel();
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndContinue");
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.Model.Should().Be(model);
            }
        }

        [TestMethod]
        public async Task ManualAddressForReprocessingSite_Post_SaveAndContinue_RedirectsCorrectly()
        {
            // Arrange
            var model = new ManualAddressForReprocessingSiteViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndContinue");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.RegulatorAddressForNotices);
            }
        }

        [TestMethod]
        public async Task ManualAddressForReprocessingSite_Post_SaveAndComeBackLater_RedirectsCorrectly()
        {
            // Arrange
            var model = new ManualAddressForReprocessingSiteViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndComeBackLater");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
            }
        }


        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Get_ReturnsViewWithModel()
        {
            // Arrange
            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.PostcodeForServiceOfNotices();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be("PostcodeForServiceOfNotices");
                viewResult.Model.Should().BeOfType<PostcodeForServiceOfNoticesViewModel>();
            }
        }


        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new PostcodeForServiceOfNoticesViewModel();
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndContinue");
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.Model.Should().Be(model);
            }
        }

        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
        {
            // Arrange
            var model = new PostcodeForServiceOfNoticesViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndContinue");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.SelectAddressForServiceOfNotices);
            }
        }

        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
        {
            // Arrange
            var model = new PostcodeForServiceOfNoticesViewModel();
            _validationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ReprocessorExporterRegistrationSession());

            // Act
            var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndComeBackLater");
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.SelectAddressForServiceOfNotices);
            }
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
