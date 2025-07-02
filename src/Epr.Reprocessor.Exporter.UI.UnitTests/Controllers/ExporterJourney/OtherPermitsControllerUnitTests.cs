using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class OtherPermitsControllerUnitTests
    {
        private Mock<ILogger<OtherPermitsController>> _loggerMock;
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
            _loggerMock = new Mock<ILogger<OtherPermitsController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _otherPermitsServiceMock = new Mock<IOtherPermitsService>();
            _configurationMock = new Mock<IConfiguration>();
        }

        private OtherPermitsController CreateController()
        {
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession());

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var controller = new OtherPermitsController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _configurationMock.Object,
                _otherPermitsServiceMock.Object
            );

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            controller.TempData = TempDataDictionary;

            return controller;
        }

        [TestMethod]
        public async Task Get_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

			var dto = new OtherPermitsDto { RegistrationId = registrationId };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Save_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("Test", "Invalid");

            var viewModel = new OtherPermitsViewModel();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Save_ValidModel_SaveAndContinue_RedirectsToPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterCheckYourAnswers, redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Save_ValidModel_SaveAndComeBackLater_RedirectsToApplicationSaved()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndComeBackLater");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ViewResult>(result);
        }

        [TestMethod]
        public async Task Save_ValidModel_UnknownButtonAction_ReturnsViewWithControllerName()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "UnknownAction");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(nameof(OtherPermitsController), viewResult.ViewName);
        }

        [TestMethod]
        public async Task Save_ConfirmAndContinue_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            Assert.IsNotNull(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(PagePaths.ExporterCheckYourAnswers, redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Save_BlankButton_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "");

            // Assert
            var redirectResult = result as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(OtherPermitsController), redirectResult.ViewName);
        }

        [TestMethod]
        public async Task Save_CheckYourAnswers_ConfirmAndContinue_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "ConfirmAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(PagePaths.ExporterPlaceholder, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_CheckYourAnswers_SaveAndContinueLater_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinueLater");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(PagePaths.ExporterPlaceholder, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_CheckYourAnswers_BlankButton_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "");

            // Assert
            var redirectResult = result as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(OtherPermitsController), redirectResult.ViewName);
        }

        [TestMethod]
        public async Task Get_CheckYourAnswers_WithViewModel()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

			var dto = new OtherPermitsDto { RegistrationId = registrationId };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.CheckYourAnswers(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Get_WhenServiceThrowsException_LogsErrorAndReturnsView()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task Get_WhenServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ReturnsAsync((OtherPermitsDto)null);

            var controller = CreateController();

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
        }

        [TestMethod]
        public async Task Post_WhenServiceThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);
            _otherPermitsServiceMock.Setup(s => s.Save(dto)).Throws(new Exception("Save failed"));

            var controller = CreateController();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<Exception>(async () =>
            {
                await controller.Post(viewModel, "SaveAndContinue");
            });
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task CheckYourAnswers_WhenServiceThrowsException_LogsErrorAndReturnsView()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            var controller = CreateController();

            // Act
            var result = await controller.CheckYourAnswers(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task CheckYourAnswers_WhenServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ReturnsAsync((OtherPermitsDto)null);

            var controller = CreateController();

            // Act
            var result = await controller.CheckYourAnswers(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
        }

    }
}