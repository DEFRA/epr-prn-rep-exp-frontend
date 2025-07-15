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
            var registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");
            var dto = new OtherPermitsDto { RegistrationId = registrationId, WasteExemptionReference = new List<string>() };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId };

            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = registrationId });

            var serviceMock = new Mock<IOtherPermitsService>();
            serviceMock.As<IBaseExporterService<OtherPermitsDto>>()
                .Setup(s => s.GetByRegistrationId(registrationId))
                .ReturnsAsync(dto);

            var controller = new OtherPermitsController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _configurationMock.Object,
                serviceMock.Object
            );

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            var result = await controller.Get();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
            Assert.AreEqual(registrationId, ((OtherPermitsViewModel)viewResult.Model).RegistrationId);
        }

        [TestMethod]
        public async Task Save_InvalidModelState_ReturnsViewWithModel()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("Test", "Invalid");

            var viewModel = new OtherPermitsViewModel();

            var result = await controller.Post(viewModel, "SaveAndContinue");

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Save_ValidModel_SaveAndContinue_RedirectsToPlaceholder()
        {
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            var result = await controller.Post(viewModel, "SaveAndContinue");

            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterCheckYourAnswersPermits, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_ValidModel_SaveAndComeBackLater_RedirectsToApplicationSaved()
        {
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            var result = await controller.Post(viewModel, "SaveAndComeBackLater");

            var applicationSavedResult = result as ViewResult;
            Assert.IsNotNull(applicationSavedResult);
            Assert.Contains("ApplicationSaved", applicationSavedResult.ViewName);
        }

        [TestMethod]
        public async Task Save_ValidModel_UnknownButtonAction_ReturnsViewWithControllerName()
        {
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            var result = await controller.Post(viewModel, "UnknownAction");

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsTrue(viewResult.ViewName.Contains("OtherPermits"));
        }

        [TestMethod]
        public async Task Save_ConfirmAndContinue_RedirectsToExporterPlaceholder()
        {
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            var result = await controller.Post(viewModel, "SaveAndContinue");

            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ExporterCheckYourAnswersPermits, redirectResult.Url);
        }

        [TestMethod]
        public async Task Save_BlankButton_ReturnsViewWithControllerName()
        {
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);

            var controller = CreateController();

            var result = await controller.Post(viewModel, "");

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsTrue(viewResult.ViewName.Contains("OtherPermits"));
        }

        [TestMethod]
        public async Task Get_WhenServiceThrowsException_LogsErrorAndReturnsView()
        {
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Test exception"));

            var controller = CreateController();

            var result = await controller.Get();

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
            var registrationId = Guid.NewGuid();
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(It.IsAny<Guid>()))
                .ReturnsAsync((OtherPermitsDto)null);

            var controller = CreateController();

            var result = await controller.Get();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(OtherPermitsViewModel));
        }
    }
}
