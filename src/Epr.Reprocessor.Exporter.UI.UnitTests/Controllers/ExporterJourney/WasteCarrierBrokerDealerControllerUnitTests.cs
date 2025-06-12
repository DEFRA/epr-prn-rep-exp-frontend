using System;
using System.Threading.Tasks;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<OtherPermitsController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IWasteCarrierBrokerDealerRefService>();
        }

        [TestMethod]
        public async Task Get_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = 1;
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
            var registrationId = 2;
            _serviceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync((WasteCarrierBrokerDealerRefDto)null);

            var controller = new WasteCarrierBrokerDealerController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _serviceMock.Object
            );

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

            // Act
            var result = await controller.Post(viewModel, "SaveAndContinue");

            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.Placeholder, redirectResult.Url);
        }
    }
}