using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared.GovUk;

namespace Epr.Reprocessor.Exporter.Tests.ViewModels.Shared.GovUk
{
    [TestClass]
    public class ErrorViewModelTests
    {
        [TestMethod]
        public void KeyProperty_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            var expectedKey = "TestKey";

            // Act
            errorViewModel.Key = expectedKey;
            var actualKey = errorViewModel.Key;

            // Assert
            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void MessageProperty_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            var expectedMessage = "TestMessage";

            // Act
            errorViewModel.Message = expectedMessage;
            var actualMessage = errorViewModel.Message;

            // Assert
            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}
