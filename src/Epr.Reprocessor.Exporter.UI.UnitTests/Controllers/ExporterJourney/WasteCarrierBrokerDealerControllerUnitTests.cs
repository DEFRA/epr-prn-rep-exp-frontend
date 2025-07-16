using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using static Epr.Reprocessor.Exporter.UI.App.Constants.Endpoints;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class WasteCarrierBrokerDealerControllerUnitTests
    {
        private Mock<ILogger<WasteCarrierBrokerDealerController>> _loggerMock;
        private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWasteCarrierBrokerDealerRefService> _serviceMock;
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();
        private readonly Mock<ISession> _session = new Mock<ISession>();
        private Mock<IConfiguration> _configurationMock;

        protected ITempDataDictionary TempDataDictionary = null!;
        private readonly Guid _registrationId = Guid.Parse("9E80DE85-1224-458E-A846-A71945E79DD3");

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<WasteCarrierBrokerDealerController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IWasteCarrierBrokerDealerRefService>();
            _configurationMock = new Mock<IConfiguration>();
        }

        private WasteCarrierBrokerDealerController CreateController()
        {
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .ReturnsAsync(new ExporterRegistrationSession { RegistrationId = _registrationId });

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _configurationMock.Object,
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
			var dto = new WasteCarrierBrokerDealerRefDto { RegistrationId = _registrationId };
            var vm = new WasteCarrierBrokerDealerRefViewModel { RegistrationId = _registrationId };

            _serviceMock.Setup(s => s.GetByRegistrationId(_registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<WasteCarrierBrokerDealerRefViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as WasteCarrierBrokerDealerRefViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewModel);

            Assert.AreEqual(vm.RegistrationId, viewModel.RegistrationId);
            Assert.AreEqual(vm.CarrierBrokerDealerPermitId, viewModel.CarrierBrokerDealerPermitId);
            Assert.AreEqual(vm.WasteCarrierBrokerDealerRegistration, viewModel.WasteCarrierBrokerDealerRegistration);
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
                _configurationMock.Object,
                _serviceMock.Object
            );
            controller.ModelState.AddModelError("Test", "Invalid");

            var viewModel = new WasteCarrierBrokerDealerRefViewModel();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml", viewResult.ViewName);
            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Post_ValidModelState_SaveAndContinue_RedirectsToNextPage()
        {
            // Arrange
            var viewModel = new WasteCarrierBrokerDealerRefViewModel
            {
                RegistrationId = _registrationId,
                WasteCarrierBrokerDealerRegistration = "ABC123"
            };

            var controller = CreateController();

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.OtherPermits, redirectResult.Url);
        }

        [TestMethod]
        public async Task Get_ServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
			_serviceMock.Setup(s => s.GetByRegistrationId(_registrationId)).ReturnsAsync((WasteCarrierBrokerDealerRefDto)null);

            var controller = CreateController();

            controller.ControllerContext.HttpContext = _httpContextMock.Object;

            // Act
            var result = await controller.Get();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("~/Views/ExporterJourney/WasteCarrierBrokerDealerReference/WasteCarrierBrokerDealerReference.cshtml", viewResult.ViewName);

            var model = viewResult.Model as WasteCarrierBrokerDealerRefViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(_registrationId, model.RegistrationId);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("Unable to retrieve Waste Carrier, Broker or Dealer Reference for registration")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );

        }


    }
}