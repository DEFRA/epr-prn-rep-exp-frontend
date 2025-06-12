namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels
{
    [TestClass]
    public class ErrorViewModelTests
    {
        [TestMethod]
        public void RequestId_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            var expectedRequestId = "12345";

            // Act
            errorViewModel.RequestId = expectedRequestId;
            var actualRequestId = errorViewModel.RequestId;

            // Assert
            Assert.AreEqual(expectedRequestId, actualRequestId);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnTrue_WhenRequestIdIsNotNullOrEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = "12345"
            };

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            Assert.IsTrue(showRequestId);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = null
            };

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            Assert.IsFalse(showRequestId);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = string.Empty
            };

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            Assert.IsFalse(showRequestId);
        }
    }
}
