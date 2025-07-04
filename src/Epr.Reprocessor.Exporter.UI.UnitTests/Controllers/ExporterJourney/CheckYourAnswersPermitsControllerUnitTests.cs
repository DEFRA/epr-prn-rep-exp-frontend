using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class CheckYourAnswersPermitsControllerUnitTests
    {
        private Mock<ILogger<CheckYourAnswersPermitsController>> _loggerMock;
        private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IOtherPermitsService> _otherPermitsServiceMock;
        private Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();
        private readonly Mock<ISession> _session = new Mock<ISession>();
        protected ITempDataDictionary TempDataDictionary = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CheckYourAnswersPermitsController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _otherPermitsServiceMock = new Mock<IOtherPermitsService>();
            _configurationMock = new Mock<IConfiguration>();
        }

        private CheckYourAnswersPermitsController CreateController()
        {
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession());

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var controller = new CheckYourAnswersPermitsController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _configurationMock.Object,
                _otherPermitsServiceMock.Object
            );

            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);
            controller.ControllerContext.HttpContext = _httpContextMock.Object;
            TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            controller.TempData = TempDataDictionary;

            return controller;
        }

        [TestMethod]
        public async Task Get_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new OtherPermitsDto { RegistrationId = registrationId, WasteExemptionReference = new List<string> { "ref1" } };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId, WasteExemptionReference = new List<string> { "ref1" } };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            Assert.AreEqual(registrationId, ((OtherPermitsViewModel)viewResult.Model).RegistrationId);
        }

        [TestMethod]
        public async Task Get_WhenWasteExceptionReferenceIsNull_CreatesWasteExceptionReferenceList()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new OtherPermitsDto { RegistrationId = registrationId, WasteExemptionReference = null };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId, WasteExemptionReference = null };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            Assert.AreEqual(1, ((OtherPermitsViewModel)viewResult.Model).WasteExemptionReference.Count);
        }

        [TestMethod]
        public async Task Get_WhenWasteExceptionReferenceIsEmpty_CreatesWasteExceptionReferenceList()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var dto = new OtherPermitsDto { RegistrationId = registrationId, WasteExemptionReference = new List<string>() };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId, WasteExemptionReference = new List<string>() };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            Assert.AreEqual(1, ((OtherPermitsViewModel)viewResult.Model).WasteExemptionReference.Count);
        }

        [TestMethod]
        public async Task Get_WhenServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ReturnsAsync((OtherPermitsDto)null);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
        }

        [TestMethod]
        public async Task Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("Test", "Invalid");

            var viewModel = new OtherPermitsViewModel();

            // Act
            var result = await controller.Post(viewModel, "ConfirmAndContinue");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Post_ConfirmAndContinue_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel { WasteExemptionReference = new List<string> { "ref1", "" } };
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);
            _otherPermitsServiceMock.Setup(m => m.Save(dto)).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "ConfirmAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterPlaceholder, redirectResult.Url);
        }

        [TestMethod]
        public async Task Post_SaveAndContinueLater_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel { WasteExemptionReference = new List<string> { "ref1", "" } };
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);
            _otherPermitsServiceMock.Setup(m => m.Save(dto)).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinueLater");

            // Assert
            var applicationSavedResult = result as ViewResult;
            Assert.IsNotNull(applicationSavedResult);
            Assert.Contains("ApplicationSaved", applicationSavedResult.ViewName);
        }

        [TestMethod]
        public async Task Post_UnknownButtonAction_ReturnsViewWithControllerName()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel { WasteExemptionReference = new List<string> { "ref1" } };
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);
            _otherPermitsServiceMock.Setup(m => m.Save(dto)).Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "UnknownAction");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.Contains("CheckYourAnswersPermits", viewResult.ViewName);
        }
    }
}
