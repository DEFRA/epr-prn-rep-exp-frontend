using System;
using System.Threading.Tasks;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<OtherPermitsController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _mapperMock = new Mock<IMapper>();
            _otherPermitsServiceMock = new Mock<IOtherPermitsService>();
        }

        private OtherPermitsController CreateController()
        {
            return new OtherPermitsController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _otherPermitsServiceMock.Object
            );
        }

        [TestMethod]
        public async Task Get_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var registrationId = 1;
            var dto = new OtherPermitsDto { RegistrationId = registrationId };
            var vm = new OtherPermitsViewModel { RegistrationId = registrationId };

            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync(dto);
            _mapperMock.Setup(m => m.Map<OtherPermitsViewModel>(dto)).Returns(vm);

            var controller = CreateController();

            // Act
            var result = await controller.Get(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(vm, viewResult.Model);
        }

        [TestMethod]
        public async Task Get_ServiceReturnsNull_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var registrationId = 2;
            _otherPermitsServiceMock.Setup(s => s.GetByRegistrationId(registrationId)).ReturnsAsync((OtherPermitsDto)null);

            var controller = CreateController();

            // Act
            var result = await controller.Get(registrationId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as OtherPermitsViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(registrationId, model.RegistrationId);
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
            Assert.AreEqual("OtherPermits", viewResult.ViewName);
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
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.Placeholder, redirectResult.Url);
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
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
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
        public async Task Save_ServiceThrowsException_LogsErrorAndRethrows()
        {
            // Arrange
            var viewModel = new OtherPermitsViewModel();
            var dto = new OtherPermitsDto();
            _mapperMock.Setup(m => m.Map<OtherPermitsDto>(viewModel)).Returns(dto);
            _otherPermitsServiceMock.Setup(s => s.Save(dto)).Throws(new Exception("Test exception"));

            var controller = CreateController();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await controller.Post(viewModel, "SaveAndContinue");
            });
            _loggerMock.Verify(
                l => l.LogError(It.IsAny<Exception>(), "Unable to save Other Permits"),
                Times.Once);
        }
    }
}