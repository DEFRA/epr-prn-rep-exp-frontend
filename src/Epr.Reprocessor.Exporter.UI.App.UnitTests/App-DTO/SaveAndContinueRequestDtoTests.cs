using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.App_DTO
{
    [TestClass]
    public class SaveAndContinueRequestDtoTests
    {
        [TestMethod]
        public void SaveAndContinueRequestDto_ShouldSetAndGetAction()
        {
            // Arrange
            var dto = new SaveAndContinueRequestDto();
            var action = "TestAction";

            // Act
            dto.Action = action;

            // Assert
            Assert.AreEqual(action, dto.Action);
        }

        [TestMethod]
        public void SaveAndContinueRequestDto_ShouldSetAndGetController()
        {
            // Arrange
            var dto = new SaveAndContinueRequestDto();
            var controller = "TestController";

            // Act
            dto.Controller = controller;

            // Assert
            Assert.AreEqual(controller, dto.Controller);
        }

        [TestMethod]
        public void SaveAndContinueRequestDto_ShouldSetAndGetParameters()
        {
            // Arrange
            var dto = new SaveAndContinueRequestDto();
            var parameters = "TestParameters";

            // Act
            dto.Parameters = parameters;

            // Assert
            Assert.AreEqual(parameters, dto.Parameters);
        }

        [TestMethod]
        public void SaveAndContinueRequestDto_ShouldSetAndGetRegistrationId()
        {
            // Arrange
            var dto = new SaveAndContinueRequestDto();
            var registrationId = 123;

            // Act
            dto.RegistrationId = registrationId;

            // Assert
            Assert.AreEqual(registrationId, dto.RegistrationId);
        }

        [TestMethod]
        public void SaveAndContinueRequestDto_ShouldSetAndGetArea()
        {
            // Arrange
            var dto = new SaveAndContinueRequestDto();
            var area = "TestArea";

            // Act
            dto.Area = area;

            // Assert
            Assert.AreEqual(area, dto.Area);
        }
    }
}
