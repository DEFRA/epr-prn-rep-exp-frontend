using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class WasteCarrierBrokerDealerControllerUnitTests
    {
        private Mock<ILogger<OtherPermitsController>> _loggerMock;
        private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWasteCarrierBrokerDealerRefService> _serviceMock;
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
            _serviceMock = new Mock<IWasteCarrierBrokerDealerRefService>();
        }

        private WasteCarrierBrokerDealerController CreateController()
        {
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession());

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
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
            var registrationId = Guid.NewGuid();
            var dto = new WasteCarrierBrokerDealerRefDto { RegistrationId = registrationId };
            var vm = new WasteCarrierBrokerDealerRefViewModel { RegistrationId = registrationId };

            _serviceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<WasteCarrierBrokerDealerRefViewModel>(dto)).Returns(vm);

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
            );

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
            );
            controller.ModelState.AddModelError("Test", "Invalid");

            var viewModel = new WasteCarrierBrokerDealerRefViewModel();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("ExporterWasteCarrierBrokerDealerReference", viewResult.ViewName);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Get_ServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync((WasteCarrierBrokerDealerRefDto)null);

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
            );

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as WasteCarrierBrokerDealerRefViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(registrationId, model.RegistrationId);
        }

        [TestMethod]
        public async Task Post_ValidModel_SaveAndContinue_RedirectsToPlaceholder()
        {
            // Arrange
            var viewModel = new WasteCarrierBrokerDealerRefViewModel();
            var dto = new WasteCarrierBrokerDealerRefDto();
            _mapperMock.Setup(m => m.Map<WasteCarrierBrokerDealerRefDto>(viewModel)).Returns(dto);

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
            );

            controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.OtherPermits, redirectResult.Url);
        }
    }
}