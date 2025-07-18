using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.Extensions.Azure;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class CheckYourAnswersForNoticeAddressControllerUnitTests
    {
        private Mock<ILogger<CheckYourAnswersForNoticeAddressController>> _loggerMock;
        private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ICheckYourAnswersForNoticeAddressService> _CheckYourAnswersForNoticeAddressServiceMock;
        private Mock<IRegistrationService> _RegistrationServiceMock;
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();
        private readonly Mock<ISession> _session = new Mock<ISession>();
        protected ITempDataDictionary TempDataDictionary = null!;


        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CheckYourAnswersForNoticeAddressController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _CheckYourAnswersForNoticeAddressServiceMock = new Mock<ICheckYourAnswersForNoticeAddressService>();
            _RegistrationServiceMock = new Mock<IRegistrationService>();
        }

        private CheckYourAnswersForNoticeAddressController CreateController()
        {
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession());

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var controller = new CheckYourAnswersForNoticeAddressController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _CheckYourAnswersForNoticeAddressServiceMock.Object,
                _RegistrationServiceMock.Object
                );

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            controller.TempData = TempDataDictionary;

            return controller;
        }

        [TestMethod]
        public async Task Get_WithAddressInSession_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

			var dto = new AddressDto {  AddressLine1 = "Some address line" };
            var vm = new CheckYourAnswersForNoticeAddressViewModel {  AddressLine1 = "Some address line" };

            _CheckYourAnswersForNoticeAddressServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<CheckYourAnswersForNoticeAddressViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = dto });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Get_WithoutAddressInSessionNorDto_ReturnsViewResult_WithEmptyViewModel()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

            var dto = new AddressDto { AddressLine1 = "Some address line" };
            var vm = new CheckYourAnswersForNoticeAddressViewModel();

            _CheckYourAnswersForNoticeAddressServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<CheckYourAnswersForNoticeAddressViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = null });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as CheckYourAnswersForNoticeAddressViewModel;
            Assert.AreEqual(vm.AddressLine1, model.AddressLine1);
        }

        [TestMethod]
        public async Task Get_WithAddressInDto_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

            var dto = new AddressDto { AddressLine1 = "Some address line" };
            var vm = new CheckYourAnswersForNoticeAddressViewModel { AddressLine1 = "Some address line" };

            _CheckYourAnswersForNoticeAddressServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<CheckYourAnswersForNoticeAddressViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = null });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Get_WithAddressNotInDtoAndNotInDb_ReturnsViewResult_WithEmptyViewModel_AndLogMsg()
        {
            // Arrange
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

            AddressDto dto = null;
            var vm = new CheckYourAnswersForNoticeAddressViewModel();

            _CheckYourAnswersForNoticeAddressServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<CheckYourAnswersForNoticeAddressViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = null });

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

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
        public async Task Save_ValidModel_ConfirmAndContinue_RedirectsToTaskListPage()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var addressDto = new AddressDto { AddressLine1 = "Some address line" };
            var controller = CreateController();

            _RegistrationServiceMock.Setup(x => x.UpdateRegistrationTaskStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateRegistrationTaskStatusDto>()))
                .Returns(Task.FromResult(true));

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = addressDto });

            // Act
            var result = await controller.Post("ConfirmAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterRegistrationTaskList, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_ValidModel_SaveAndContinueLater_RedirectsToRedirectsToApplicationSaved()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var addressDto = new AddressDto { AddressLine1 = "Some address line" };
            var controller = CreateController();

            _RegistrationServiceMock.Setup(x => x.UpdateRegistrationTaskStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateRegistrationTaskStatusDto>()))
                .Returns(Task.FromResult(true));

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = addressDto });

            // Act
            var result = await controller.Post("SaveAndContinueLater");

            // Assert
            var redirectResult = result as ViewResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("~/Views/Shared/ApplicationSaved.cshtml", redirectResult.ViewName);
        }

        [TestMethod]
        public async Task Save_ValidModel_BlankButton_RedirectsToExporterPlaceholder()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var addressDto = new AddressDto { AddressLine1 = "Some address line" };
            var controller = CreateController();

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId, LegalAddress = addressDto });

            // Act
            var result = await controller.Post("");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterPlaceholder, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_WhenServiceThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var dto = new AddressDto();
            var registrationId = Guid.NewGuid();
            _CheckYourAnswersForNoticeAddressServiceMock.Setup(s => s.Save(registrationId, dto)).Throws(new Exception("Save failed"));

            var controller = CreateController();

            // Act & Assert
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
            {
                await controller.Post("SaveAndContinue");
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
    }
}